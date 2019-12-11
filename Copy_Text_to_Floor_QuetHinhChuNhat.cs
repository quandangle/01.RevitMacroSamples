  public void Copy_Text_to_Floor_QuetHinhChuNhat () 
        {
            Application app = this.Application;
            Document doc = this.ActiveUIDocument.Document;
            UIDocument uidoc = this.ActiveUIDocument;

            // Ch?n sàn c?n modify point và l?y v? t?t c? các d?nh c?a nó
            var rf = uidoc.Selection.PickObject (ObjectType.Element, new FloorSelectionFilter (), "CH?N SÀN MU?N MODIFY POINT");
            Floor floor = doc.GetElement (rf) as Floor;
            SlabShapeEditor slabShapeEditor = floor.SlabShapeEditor;
            SlabShapeVertexArray verArr = slabShapeEditor.SlabShapeVertices;
            //TaskDialog.Show("x",verArr.Size.ToString());

            List<XYZ> allVertexOfFloor = new List<XYZ> ();
            foreach (SlabShapeVertex shapeVertex in verArr) {
                allVertexOfFloor.Add (shapeVertex.Position);
            }
            //TaskDialog.Show("x",allVertexOfFloor.Count.ToString());

          
            Transaction trans = new Transaction (doc, "Add Point");
				trans.Start ();
            while (true) {
                try {
                    // Ch?n text d? l?y cao d?
                    var rText = uidoc.Selection.PickObject (ObjectType.Element, new TextSelectionFilter (), "PICK TEXT");
                    var e = doc.GetElement (rText);
                    TextNote txtNote = e as TextNote;
                    string s = txtNote.Text;
                    s = s.Trim ();
                    double caoDo = Convert.ToDouble (s);

                    // Quét vùng có cùng cao d? -> l?y point c?a sàn n?m trong vùng dó
                    PickedBox pick = uidoc.Selection.PickBox (PickBoxStyle.Crossing, "QUÉT HÌNH CH? NH?T T? DU?I - PH?I LÊN TRÊN - TRÁI");
                    BoundingBoxXYZ bb = new BoundingBoxXYZ ();
                    bb.Max = new XYZ (pick.Max.X, pick.Max.Y, 1000);
                    bb.Min = new XYZ (pick.Min.X, pick.Min.Y, -1000);
                    // TaskDialog.Show("x",bb.Max+"\n"+bb.Min);

                    /*   foreach (SlabShapeVertex shapeVertex in verArr) 
        	      {
        	        XYZ pt = shapeVertex.Position;
        	        //TaskDialog.Show("x",pt.ToString());
        	       	if (BoundingBoxXyzContains(bb,pt)) 
        	       	{
        	       		//TaskDialog.Show("x",pt.ToString());
        	       		slabShapeEditor.ModifySubElement(shapeVertex,DLQUnitUtils.MeterToFeet(caoDo));
        	       	}
        	       }

				*/

                    foreach (XYZ pt in allVertexOfFloor) 
                    {
                        if (DLQUtils.BoundingBoxXyzContains2 (bb, pt)) 
                        {
                            //TaskDialog.Show("x",pt.ToString());
                            try
                            {

                            	slabShapeEditor.DrawPoint (new XYZ (pt.X, pt.Y, DLQUnitUtils.MeterToFeet (caoDo)));
                            	
                            	   TextNoteOptions options = new TextNoteOptions (new ElementId (361));
                            XYZ poCurrentView = new XYZ (pt.X, pt.Y, doc.ActiveView.GenLevel.Elevation);
                            TextNote.Create (doc, doc.ActiveView.Id, poCurrentView,
                                0.03108, Math.Round (caoDo, 2).ToString (), options);

                            	
                            } catch (Exception)
                            {
                            	continue;
                            	 // break;
                            }
                        }
                    }
                } 
            	catch (Exception) 
            	{
                    //TaskDialog.Show ("Note", "Error");
                     trans.Commit ();
                    break;
                }
            }
            //trans.Commit ();
        }
