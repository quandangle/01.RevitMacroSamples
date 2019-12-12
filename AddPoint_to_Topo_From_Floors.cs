 public void AddPoint_to_Topo_From_Floors () 
        {
            Application app = this.Application;
            Document doc = this.ActiveUIDocument.Document;
            UIDocument uidoc = this.ActiveUIDocument;

            // code
            
            if (uidoc.Selection.GetElementIds ().Count == 0) {
                TaskDialog.Show ("x", "CHỌN FLOOR TRƯỚC KHI CHẠY LỆNH. FLOOR PHẢI CÓ ĐÁY PHẲNG");
                return;
            }

            List<Floor> floors = new FilteredElementCollector (doc, uidoc.Selection.GetElementIds ())
                .OfClass (typeof (Floor))
                .Cast<Floor> ()
                .ToList ();
            
            Reference rt = uidoc.Selection.PickObject (ObjectType.Element,
                                                 new TopographySurfaceSelectionFilter (), 
                                                 "CHỌN Topography CẦN THÊM POINT");
            TopographySurface ts = doc.GetElement (rt) as TopographySurface;        


            #region Lấy về list các đỉnh của các Floors cần add point vào Topo

            Options op = new Options ();
            Solid solid = null;
            List<XYZ> points = new List<XYZ> ();

            foreach (Floor fl in floors) 
            {
                GeometryElement geoElement = fl.get_Geometry (op);
                foreach (GeometryObject geomObj in geoElement) {
                    if (geomObj is Solid) {
                        solid = (Solid) geomObj;
                        if (solid.Volume > 0) break;
                    }
                }

                if (solid == null) continue;

                foreach (Face f in solid.Faces) {
                    if (f is PlanarFace && f.ComputeNormal (new UV ()).IsAlmostEqualTo (XYZ.BasisZ)) {
                        points.AddRange (f.Triangulate ().Vertices);

                    }
                }
            }
            
            #endregion
            
            
            #region Thêm point vào Topo
            
            using (TopographyEditScope tsEditScope = new TopographyEditScope (doc, "AddPoint_to_Topo_From_Floor_All"))
            {
                tsEditScope.Start (ts.Id);
                using (Transaction trans = new Transaction (doc, "AddPoint_to_Topo_From_Floor_All"))
                 {
                 	trans.Start ();
                 	ts.AddPoints(points);
                 	trans.Commit ();
                 }
                tsEditScope.Commit (new DeleteWarningSuper ());
            }

            uidoc.Selection.SetElementIds (new List<ElementId>(){ts.Id});
            
            #endregion
            
        }
