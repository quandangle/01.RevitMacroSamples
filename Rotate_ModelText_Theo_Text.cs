/*
 * Created by SharpDevelop.
 * User: quan.dangle
 * Date: 12/6/2019
 * Time: 10:05 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace DLQSample {
    [Autodesk.Revit.Attributes.Transaction (Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.DB.Macros.AddInId ("DE7B944C-D924-4C81-A7C3-12FE5DA7A7B9")]
    public partial class ThisDocument {
        private void Module_Startup (object sender, EventArgs e) {

        }

        private void Module_Shutdown (object sender, EventArgs e) {

        }

        #region Revit Macros generated code
        private void InternalStartup () {
            this.Startup += new System.EventHandler (Module_Startup);
            this.Shutdown += new System.EventHandler (Module_Shutdown);
        }
        #endregion

        public void Rotate_ModelText_theo_Text () {
            Application app = this.Application;
            Document doc = this.ActiveUIDocument.Document;
            UIDocument uidoc = this.ActiveUIDocument;

            #region Ch?n Text & ModelText d? move lên sàn

            List<ElementId> textIds = uidoc.Selection.GetElementIds ().ToList ();
            if (!textIds.Any ()) {
                TaskDialog.Show ("x", "Ch?n Text & ModelText");
                return;
            }
            List<ModelText> selectedModelText = textIds.Select (id => doc.GetElement (id))
                .Where (e => e is ModelText)
                .Cast<ModelText> ()
                .ToList ();
            if (!selectedModelText.Any ()) {
                TaskDialog.Show ("x", "Ch?n ModelText");
                return;
            }

            List<TextNote> selectedText = textIds.Select (id => doc.GetElement (id))
                .Where (e => e is TextNote)
                .Cast<TextNote> ()
                .ToList ();
            if (!selectedText.Any ()) {
                TaskDialog.Show ("x", "Ch?n TextNote");
                return;
            }

            #endregion

            BoundingBoxXYZ bbModelText = selectedModelText[0].get_BoundingBox (doc.ActiveView);
            BoundingBoxXYZ bbText = selectedText[0].get_BoundingBox (doc.ActiveView);

            XYZ max = new XYZ (bbModelText.Max.X, bbModelText.Max.Y, 0);
            XYZ min = new XYZ (bbModelText.Min.X, bbModelText.Min.Y, 0);
            XYZ dirModelText = max.Subtract (min);

            //max = new XYZ(bbText.Max.X,bbText.Max.Y,0);
            //min = new XYZ(bbText.Min.X,bbText.Min.Y,0);
            //XYZ dirText = max.Subtract(min);

            //double gocXoay = dirModelText.AngleTo(dirText);

            double x = selectedText[0].BaseDirection.X;
            double y = selectedText[0].BaseDirection.Y;
            XYZ dirText = new XYZ (x, y, 0);
            double gocXoay = 0;
            if (x < 0) {
                x = -x;
                y = -y;
                dirText = new XYZ (x, y, 0);
                gocXoay = dirModelText.AngleTo (dirText) - Math.PI;
            } else {
                gocXoay = dirModelText.AngleTo (dirText);
            }

            // TaskDialog.Show("x",DLQUnitUtils.RadiansToDegrees(gocXoay).ToString());
            Line line = Line.CreateUnbound (selectedText[0].Coord, XYZ.BasisZ);

            Transaction trans = new Transaction (doc, "Rotate_ModelText_theo_Text");
            trans.Start ();

            /*
			if (selectedText[0].BaseDirection.X<0||selectedText[0].BaseDirection.Y<0)
        	{
        		ElementTransformUtils.RotateElement(doc,selectedModelText[0].Id,line, gocXoay);
        	}
        	else	
        	{
        		ElementTransformUtils.RotateElement(doc,selectedModelText[0].Id,line,-gocXoay);
        	}
        	
        	*/

            ElementTransformUtils.RotateElement (doc, selectedModelText[0].Id, line, -gocXoay);

            trans.Commit ();

        }
    }
}
