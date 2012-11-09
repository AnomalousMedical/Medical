/******************************************************************************\
* Copyright (C) 2012 Leap Motion, Inc. All rights reserved.                    *
* NOTICE: This developer release of Leap Motion, Inc. software is confidential *
* and intended for very limited distribution. Parties using this software must *
* accept the SDK Agreement prior to obtaining this software and related tools. *
* This software is subject to copyright.                                       *
\******************************************************************************/

#include "StdAfx.h"

#include <iostream>

typedef void (*LeapManagedCallback)(const Leap::Controller* controller);

class ManagedListener : public Leap::Listener 
{
  public:
	ManagedListener(LeapManagedCallback onInitDelegate, LeapManagedCallback onConnectDelegate, LeapManagedCallback onDisconnectDelegate, LeapManagedCallback onFrameDelegate)
		:onInitDelegate(onInitDelegate),
		onConnectDelegate(onConnectDelegate),
		onDisconnectDelegate(onDisconnectDelegate),
		onFrameDelegate(onFrameDelegate)
	{

	}

	virtual ~ManagedListener()
	{

	}

    virtual void onInit(const Leap::Controller& controller)
	{
		onInitDelegate(&controller);
	}

    virtual void onConnect(const Leap::Controller& controller)
	{
		onConnectDelegate(&controller);
	}

    virtual void onDisconnect(const Leap::Controller& controller)
	{
		onDisconnectDelegate(&controller);
	}

    virtual void onFrame(const Leap::Controller& controller)
	{
		onFrameDelegate(&controller);
	}

private:
	LeapManagedCallback onInitDelegate;
    LeapManagedCallback onConnectDelegate;
    LeapManagedCallback onDisconnectDelegate;
    LeapManagedCallback onFrameDelegate;
};

//void ManagedListener::onInit(const Leap::Controller& controller) {
//  std::cout << "Initialized" << std::endl;
//}
//
//void ManagedListener::onConnect(const Leap::Controller& controller) {
//  std::cout << "Connected" << std::endl;
//}
//
//void ManagedListener::onDisconnect(const Leap::Controller& controller) {
//  std::cout << "Disconnected" << std::endl;
//}
//
//void ManagedListener::onFrame(const Leap::Controller& controller) {
//  // Get the most recent frame and report some basic information
//  const Leap::Frame frame = controller.frame();
//  const std::vector<Leap::Hand>& hands = frame.hands();
//  const size_t numHands = hands.size();
//  std::cout << "Frame id: " << frame.id()
//            << ", timestamp: " << frame.timestamp()
//            << ", hands: " << numHands << std::endl;
//
//  if (numHands >= 1) {
//    // Get the first hand
//    const Leap::Hand& hand = hands[0];
//
//    // Check if the hand has any fingers
//    const std::vector<Leap::Finger>& fingers = hand.fingers();
//    const size_t numFingers = fingers.size();
//    if (numFingers >= 1) {
//      // Calculate the hand's average finger tip position
//      Leap::Vector pos(0, 0, 0);
//      for (size_t i = 0; i < numFingers; ++i) {
//        const Leap::Finger& finger = fingers[i];
//        const Leap::Ray& tip = finger.tip();
//        pos.x += tip.position.x;
//        pos.y += tip.position.y;
//        pos.z += tip.position.z;
//      }
//      pos = Leap::Vector(pos.x/numFingers, pos.y/numFingers, pos.z/numFingers);
//      std::cout << "Hand has " << numFingers << " fingers with average tip position"
//                << " (" << pos.x << ", " << pos.y << ", " << pos.z << ")" << std::endl;
//    }
//
//    // Check if the hand has a palm
//    const Leap::Ray* palmRay = hand.palm();
//    if (palmRay != NULL) {
//      // Get the palm position and wrist direction
//      Leap::Vector palm = palmRay->position;
//      Leap::Vector wrist = palmRay->direction;
//      std::string direction = "";
//      if (wrist.x > 0)
//        direction = "left";
//      else
//        direction = "right";
//      std::cout << "Hand is pointing to the " << direction << " with palm position"
//                << " (" << palm.x << ", " << palm.y << ", " << palm.z << ")" << std::endl;
//    }
//  }
//}

extern "C" _AnomalousExport ManagedListener* ManagedListener_Create(LeapManagedCallback onInitDelegate, LeapManagedCallback onConnectDelegate, LeapManagedCallback onDisconnectDelegate, LeapManagedCallback onFrameDelegate)
{
	return new ManagedListener(onInitDelegate, onConnectDelegate, onDisconnectDelegate, onFrameDelegate);
}

extern "C" _AnomalousExport void ManagedListener_Delete(ManagedListener* managedListener)
{
	delete managedListener;
}