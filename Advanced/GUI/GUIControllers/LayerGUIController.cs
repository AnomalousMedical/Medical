using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using ComponentFactory.Krypton.Ribbon;

namespace Medical.GUI
{
    public class LayerGUIController
    {
        private KryptonRibbonTab layerTab;
        KryptonRibbonGroup layerGroup;

        public LayerGUIController(AdvancedForm advancedForm)
        {
            //layerTab = advancedForm.layersTab;
            layerGroup = new KryptonRibbonGroup();
            layerGroup.TextLine1 = "Layers";
            //layerTab.Groups.Add(layerGroup);
        }

        public void sceneLoaded(SimScene scene)
        {
            layerGroup.Items.Clear();
            KryptonRibbonGroupTriple triple = null;
            foreach (TransparencyGroup group in TransparencyController.getGroupIter())
            {
                if (triple == null || triple.Items.Count > 2)
                {
                    triple = new KryptonRibbonGroupTriple();
                    layerGroup.Items.Add(triple);
                }
                KryptonRibbonGroupButton button = new KryptonRibbonGroupButton();
                button.TextLine1 = group.Name.ToString();
                triple.Items.Add(button);
            }
        }

        public void sceneUnloading(SimScene scene)
        {
            
        }
    }
}
