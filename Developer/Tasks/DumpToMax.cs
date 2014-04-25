using Engine;
using Engine.ObjectManagement;
using Medical;
using Medical.GUI;
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

                            stream.WriteLine("trans.add \"{0}\" \"point3 {1} {2} {3}\"", simObject.Name, 
                                simObject.Translation.x, 
                                simObject.Translation.z * -1.0f, 
                                simObject.Translation.y);

                            Vector3 euler = simObject.Rotation.getEuler();

                            stream.WriteLine("rots.add \"{0}\" \"eulerAngles {1} {2} {3}\"", simObject.Name,
                                (Degree)new Radian(euler.z),
                                (Degree)new Radian(euler.y * -1.0f),
                                (Degree)new Radian(euler.x));
                        }
                        stream.Write(scriptEnd);
                    }
                }
                fireItemClosed();
            });
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

function UpdateLocation meshObj = 
(
	 if trans.containsKey meshObj.Name then 
	(
		command = trans.lookup meshObj.Name
		meshObj.pos = execute command
	)
)

function UpdateRotation meshObj = 
(
	if rots.containsKey meshObj.Name then 
	(
 		command = rots.lookup meshObj.Name
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
