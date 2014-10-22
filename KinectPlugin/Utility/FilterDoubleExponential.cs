using Engine;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectPlugin
{
    /// <summary>
    /// Based on
    /// https://social.msdn.microsoft.com/Forums/en-US/045b058a-ae3a-4d01-beb6-b756631b4b42/joint-smoothing-code-c?forum=kinectv2sdk
    /// not sure if actually working.
    /// </summary>
    class FilterDoubleExponential
    {
        struct TransformSmoothParameters
        {
            public float fSmoothing;             // [0..1], lower values closer to raw data
            public float fCorrection;            // [0..1], lower values slower to correct towards the raw data
            public float fPrediction;            // [0..n], the number of frames to predict into the future
            public float fJitterRadius;          // The radius in meters for jitter reduction
            public float fMaxDeviationRadius;    // The maximum radius in meters that filtered positions are allowed to deviate from raw data
        }

        class FilterDoubleExponentialData
        {
            public Vector3 m_vRawPosition;
            public Vector3 m_vFilteredPosition;
            public Vector3 m_vTrend;
            public uint m_dwFrameCount;
        };

        const int JointType_Count = 25;

        public FilterDoubleExponential() { Init(); }

        public void Init(float fSmoothing = 0.25f, float fCorrection = 0.25f, float fPrediction = 0.25f, float fJitterRadius = 0.03f, float fMaxDeviationRadius = 0.05f)
        {
            Reset(fSmoothing, fCorrection, fPrediction, fJitterRadius, fMaxDeviationRadius);
        }

        public void Reset(float fSmoothing = 0.25f, float fCorrection = 0.25f, float fPrediction = 0.25f, float fJitterRadius = 0.03f, float fMaxDeviationRadius = 0.05f)
        {
            m_fMaxDeviationRadius = fMaxDeviationRadius; // Size of the max prediction radius Can snap back to noisy data when too high
            m_fSmoothing = fSmoothing;                   // How much smothing will occur.  Will lag when too high
            m_fCorrection = fCorrection;                 // How much to correct back from prediction.  Can make things springy
            m_fPrediction = fPrediction;                 // Amount of prediction into the future to use. Can over shoot when too high
            m_fJitterRadius = fJitterRadius;             // Size of the radius where jitter is removed. Can do too much smoothing when too high

            m_pFilteredJoints = new Dictionary<JointType, Vector3>();// new Vector3[JointType_Count];
            m_pHistory = new Dictionary<JointType, FilterDoubleExponentialData>();// new FilterDoubleExponentialData[JointType_Count];
            foreach(JointType value in Enum.GetValues(typeof(JointType)))
            {
                m_pHistory.Add(value, new FilterDoubleExponentialData());
            }
        }

        public void Update(Body pBody)
        {
             //assert( pBody );

            // Check for divide by zero. Use an epsilon of a 10th of a millimeter
            m_fJitterRadius = Math.Max(0.0001f, m_fJitterRadius);

            TransformSmoothParameters SmoothingParams = new TransformSmoothParameters();

            foreach(var joint in pBody.Joints.Values)
            {
                SmoothingParams.fSmoothing      = m_fSmoothing;
                SmoothingParams.fCorrection     = m_fCorrection;
                SmoothingParams.fPrediction     = m_fPrediction;
                SmoothingParams.fJitterRadius   = m_fJitterRadius;
                SmoothingParams.fMaxDeviationRadius = m_fMaxDeviationRadius;

                // If inferred, we smooth a bit more by using a bigger jitter radius
                if ( joint.TrackingState == TrackingState.Inferred )
                {
                    SmoothingParams.fJitterRadius       *= 2.0f;
                    SmoothingParams.fMaxDeviationRadius *= 2.0f;
                }

                Update(joint, SmoothingParams);
            }
        }

        public void Update(Joint[] joints)
        {
            //Check for divide by zero. Use an epsilon of a 10th of a millimeter
            m_fJitterRadius = Math.Max(0.0001f, m_fJitterRadius);

            TransformSmoothParameters SmoothingParams = new TransformSmoothParameters();
            foreach(var joint in joints)
            {
                SmoothingParams.fSmoothing = m_fSmoothing;
                SmoothingParams.fCorrection = m_fCorrection;
                SmoothingParams.fPrediction = m_fPrediction;
                SmoothingParams.fJitterRadius = m_fJitterRadius;
                SmoothingParams.fMaxDeviationRadius = m_fMaxDeviationRadius;

                // If inferred, we smooth a bit more by using a bigger jitter radius
                if (joint.TrackingState == TrackingState.Inferred)
                {
                    SmoothingParams.fJitterRadius *= 2.0f;
                    SmoothingParams.fMaxDeviationRadius *= 2.0f;
                }

                Update(joint, SmoothingParams);
            }
        }

        public IReadOnlyDictionary<JointType, Vector3> FilteredJoints
        {
            get
            {
                return m_pFilteredJoints;
            }
        }

        private Dictionary<JointType, Vector3> m_pFilteredJoints;
        private Dictionary<JointType, FilterDoubleExponentialData> m_pHistory;
        private float m_fSmoothing;
        private float m_fCorrection;
        private float m_fPrediction;
        private float m_fJitterRadius;
        private float m_fMaxDeviationRadius;

        private void Update(Joint joint, TransformSmoothParameters smoothingParams)
        {
            FilterDoubleExponentialData history = m_pHistory[joint.JointType];

            Vector3 vPrevRawPosition = history.m_vRawPosition;
            Vector3 vPrevFilteredPosition = history.m_vFilteredPosition;
            Vector3 vPrevTrend = history.m_vTrend;
            Vector3 vRawPosition = new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z);
            Vector3 vFilteredPosition;
            Vector3 vPredictedPosition;
            Vector3 vDiff;
            Vector3 vTrend;
            //Vector3 vLength;
            float fDiff;
            bool bJointIsValid = JointPositionIsValid(vRawPosition);

            // If joint is invalid, reset the filter
            if (!bJointIsValid)
            {
                history.m_dwFrameCount = 0;
            }

            // Initial start values
            if (history.m_dwFrameCount == 0)
            {
                vFilteredPosition = vRawPosition;
                vTrend = Vector3.Zero;
                history.m_dwFrameCount++;
            }
            else if (history.m_dwFrameCount == 1)
            {
                vFilteredPosition = (vRawPosition + vPrevRawPosition) * 0.5f;// XMVectorScale(XMVectorAdd(vRawPosition, vPrevRawPosition), 0.5f);
                vDiff = vFilteredPosition - vPrevFilteredPosition;// XMVectorSubtract(vFilteredPosition, vPrevFilteredPosition);
                vTrend =  (vDiff * smoothingParams.fCorrection) + (vPrevTrend * (1.0f - smoothingParams.fCorrection));// XMVectorAdd(XMVectorScale(vDiff, smoothingParams.fCorrection), XMVectorScale(vPrevTrend, 1.0f - smoothingParams.fCorrection));
                history.m_dwFrameCount++;
            }
            else
            {
                // First apply jitter filter
                vDiff = vRawPosition - vPrevFilteredPosition;// XMVectorSubtract(vRawPosition, vPrevFilteredPosition);
                //vLength = XMVector3Length(vDiff);
                fDiff = Math.Abs(vDiff.length());// fabs(XMVectorGetX(vLength));

                if (fDiff <= smoothingParams.fJitterRadius)
                {
                    vFilteredPosition = (vRawPosition * (fDiff / smoothingParams.fJitterRadius)) + (vPrevFilteredPosition * (1.0f - fDiff / smoothingParams.fJitterRadius));
                        //XMVectorAdd(XMVectorScale(vRawPosition, fDiff / smoothingParams.fJitterRadius), XMVectorScale(vPrevFilteredPosition, 1.0f - fDiff / smoothingParams.fJitterRadius));
                }
                else
                {
                    vFilteredPosition = vRawPosition;
                }

                // Now the double exponential smoothing filter
                vFilteredPosition = (vFilteredPosition * (1.0f - smoothingParams.fSmoothing)) + ((vPrevFilteredPosition + vPrevTrend) * smoothingParams.fSmoothing);
                    //XMVectorAdd(XMVectorScale(vFilteredPosition, 1.0f - smoothingParams.fSmoothing), XMVectorScale(XMVectorAdd(vPrevFilteredPosition, vPrevTrend), smoothingParams.fSmoothing));


                vDiff = vFilteredPosition - vPrevFilteredPosition;// XMVectorSubtract(vFilteredPosition, vPrevFilteredPosition);
                vTrend = (vDiff * smoothingParams.fCorrection) + (vPrevTrend * (1.0f - smoothingParams.fCorrection));
                // XMVectorAdd(XMVectorScale(vDiff, smoothingParams.fCorrection), XMVectorScale(vPrevTrend, 1.0f - smoothingParams.fCorrection));
            }

            // Predict into the future to reduce latency
            vPredictedPosition = vFilteredPosition + (vTrend * smoothingParams.fPrediction);// XMVectorAdd(vFilteredPosition, XMVectorScale(vTrend, smoothingParams.fPrediction));

            // Check that we are not too far away from raw data
            vDiff = vPredictedPosition - vRawPosition;// XMVectorSubtract(vPredictedPosition, vRawPosition);
            //vLength = XMVector3Length(vDiff);
            fDiff = Math.Abs(vDiff.length());// fabs(XMVectorGetX(vLength));

            if (fDiff > smoothingParams.fMaxDeviationRadius)
            {
                vPredictedPosition = (vPredictedPosition * (smoothingParams.fMaxDeviationRadius / fDiff)) + (vRawPosition * (1.0f - smoothingParams.fMaxDeviationRadius / fDiff));
                    //XMVectorAdd(XMVectorScale(vPredictedPosition, smoothingParams.fMaxDeviationRadius / fDiff), XMVectorScale(vRawPosition, 1.0f - smoothingParams.fMaxDeviationRadius / fDiff));
            }

            // Save the data from this frame
            history.m_vRawPosition = vRawPosition;
            history.m_vFilteredPosition = vFilteredPosition;
            history.m_vTrend = vTrend;

            // Output the data
            m_pFilteredJoints[joint.JointType] = vPredictedPosition;
            //m_pFilteredJoints[JointID] = vPredictedPosition;
            //m_pFilteredJoints[JointID] = XMVectorSetW(m_pFilteredJoints[JointID], 1.0f);
        }

        private bool JointPositionIsValid(Vector3 vJointPosition)
        {
            return (vJointPosition.x != 0.0f ||
                    vJointPosition.y != 0.0f ||
                    vJointPosition.z != 0.0f);
        }
    };
}
