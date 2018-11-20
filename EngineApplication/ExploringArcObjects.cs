using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;

namespace EngineApplication
{
    public class ExploringArcObjects
    {
        public IFeatureLayer TaoFeatureLayer()
        {
            ILayer layer;
            IFeatureLayer featureLayer = new FeatureLayerClass();
            string tenLayer = featureLayer.Name;
            //或者
            layer = (ILayer)featureLayer;
            tenLayer = layer.Name;

            IFeatureClass featureClass = featureLayer.FeatureClass;

            IMap map = new MapClass();
            ISelection selection = map.FeatureSelection;

            // 从要素类的形状字段中获取GeometryDef
            String shapeFieldName = featureClass.ShapeFieldName;
            int shapeFieldIndex = featureClass.FindField(shapeFieldName);
            IFields fields = featureClass.Fields;
            IField shapeField = fields.get_Field(shapeFieldIndex);
            IGeometryDef geometryDef = shapeField.GeometryDef;



            return featureLayer;
        }
    }
}
