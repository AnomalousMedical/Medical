using Engine;
using Engine.ObjectManagement;
using Medical;
using Medical.GUI;
using OgrePlugin;
using OgreWrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Developer
{
    class DumpToMax : Task
    {
        private MedicalController medicalController;

        public DumpToMax(MedicalController medicalController)
            : base("Developer.DumpToMax", "Dump Positions to 3ds Max", CommonResources.NoIcon, "Developer")
        {
            this.ShowOnTaskbar = false;
            this.medicalController = medicalController;
        }

        public override void clicked(TaskPositioner taskPositioner)
        {
            HashSet<String> foundMeshes = new HashSet<string>();

            FileSaveDialog saveDialog = new FileSaveDialog(MainWindow.Instance, "Dump Positions to 3ds Max", Environment.CurrentDirectory, "AnomalousMedicalSimObjects.ms", "MaxScript (*.ms)|*.ms");
            saveDialog.showModal((result, path) =>
            {
                if (result == NativeDialogResult.OK)
                {
                    using (StreamWriter stream = new StreamWriter(File.Open(path, FileMode.Create, FileAccess.Write)))
                    {
                        stream.Write(ScriptBegin);
                        foreach(SimObject simObject in medicalController.SimObjects)
                        {
                            String meshName = findMeshName(simObject);
                            if(foundMeshes.Contains(meshName))
                            {
                                meshName = null; //Already found
                            }
                            else
                            {
                                foundMeshes.Add(meshName);
                            }

                            String name = simObject.Name;

                            String transStr = String.Format("point3 {0} {1} {2}",
                                simObject.Translation.x,
                                simObject.Translation.z * -1.0f,
                                simObject.Translation.y);

                            stream.WriteLine("trans.add \"{0}\" \"{1}\"", name, transStr);
                            if(meshName != null)
                            {
                                stream.WriteLine("meshTrans.add \"{0}\" \"{1}\"", meshName, transStr);
                            }

                            Vector3 euler = simObject.Rotation.getEuler();
                            String rotString = String.Format("eulerAngles {0} {1} {2}", 
                                (Degree)new Radian(euler.z),
                                (Degree)new Radian(euler.y * -1.0f),
                                (Degree)new Radian(euler.x));

                            stream.WriteLine("rots.add \"{0}\" \"{1}\"", name, rotString);
                            if(meshName != null)
                            {
                                stream.WriteLine("meshRots.add \"{0}\" \"{1}\"", meshName, rotString);
                            }
                        }
                        stream.Write(scriptEnd);
                    }
                }
                fireItemClosed();
            });
        }

        private static String findMeshName(SimObject simObject)
        {
            String name = null;
            foreach (var element in simObject.getElementIter())
            {
                SceneNodeElement sceneElement = element as SceneNodeElement;
                if (sceneElement != null)
                {
                    foreach (var movable in sceneElement.MovableObjects)
                    {
                        Entity entity = movable as Entity;
                        if (entity != null)
                        {
                            using (MeshPtr mesh = entity.getMesh())
                            {
                                String meshName = mesh.Value.getName();
                                if (meshName != null)
                                {
                                    name = Path.GetFileNameWithoutExtension(meshName);
                                    break;
                                }
                            }
                        }
                    }
                    break;
                }
            }
            return name;
        }

        public override bool Active
        {
            get
            {
                return false;
            }
        }

        const string ScriptBegin = @"struct mxsHashTable
(
    _hashTable = dotNetObject ""System.Collections.HashTable"",

    fn add key value = (
        _hashTable.Add(dotNetObject ""System.String"" (key as string)) (dotNetObject ""System.String"" (value as string))
    ),

    fn clear = (
        _hashTable.Clear()
    ),

    fn containsKey key = (
        _hashTable.ContainsKey (key as string)
    ),

    fn containsValue value = (
        _hashTable.ContainsValue (value as string)
    ),

    fn remove key = (
        _hashTable.Remove (key as string)
    ),

    fn lookup key = (
        _hashTable.item[(dotNetObject ""System.String"" (key as string))]
    ),

    fn modify key value = (
        remove key
        add key value
    ),

    fn size = (
        _hashTable.count
    )
)

global trans = mxsHashTable()
global rots = mxsHashTable()

global meshTrans = mxsHashTable()
global meshRots = mxsHashTable()

function UpdateLocation meshObj = 
(
	if trans.containsKey meshObj.Name then 
	(
		command = trans.lookup meshObj.Name
	)

	else if meshTrans.containsKey meshObj.Name then 
	(
		command = meshTrans.lookup meshObj.Name
	)

    if command != undefined then
    (
        meshObj.pos = execute command
    )
)

function UpdateRotation meshObj = 
(
	if rots.containsKey meshObj.Name then 
	(
 		command = rots.lookup meshObj.Name
    )

	else if meshRots.containsKey meshObj.Name then 
	(
 		command = meshRots.lookup meshObj.Name
    )

    if command != undefined then
    (
 		eulerRot = execute command

		local translateMat = transMatrix meshObj.transform.pos
		local scaleMat = scaleMatrix meshObj.transform.scale
		meshObj.transform = scaleMat * translateMat
	   
		-- Perform each axis rotation individually
		rotate meshObj (angleaxis eulerRot.x [1,0,0])
		rotate meshObj (angleaxis eulerRot.y [0,1,0])
		rotate meshObj (angleaxis eulerRot.z [0,0,1])		
	)
)

function MoveNode node =
(
		exportName = node.Name
		if(exportName != undefined) then
		(
			UpdateLocation node
			UpdateRotation node
		)
)

function RecursiveMoveNode node =
(
	MoveNode node

	for child in node.children do
		RecursiveMoveNode child
)

function MoveAllNodes parentNode = 
(	
	for node in parentNode do
			RecursiveMoveNode node
)

";

    String scriptEnd = @"
in coordsys world
(
	MoveAllNodes(rootnode.children)
)";
    }
}
