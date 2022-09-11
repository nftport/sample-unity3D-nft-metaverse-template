using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLTFast;
using GLTFast.Export;
using GLTFast.Logging;
using UnityEngine.Events;


public class ExportGameobject : MonoBehaviour
{
   
    private UnityAction<bool> onComplete;
    
    public ExportGameobject OnComplete(UnityAction<bool> action)
    {
        this.onComplete = action;
        return this;
    }

    //via https://github.com/atteneder/glTFast/blob/main/Documentation~/ExportRuntime.md
    public async void AdvancedExport(string Savepath , GameObject[] toExport) {
        
        // CollectingLogger lets you programatically go through
        // errors and warnings the export raised
        var logger = new CollectingLogger();

        // ExportSettings allow you to configure the export
        // Check its source for details
        var exportSettings = new ExportSettings {
            format = GltfFormat.Binary,
            fileConflictResolution = FileConflictResolution.Overwrite,
            imageDestination = ImageDestination.MainBuffer
        };

        var gameObjectexportsettings = new GameObjectExportSettings(
            
            
            
            );
        
        var matExport = new StandardMaterialExport();

        // GameObjectExport lets you create glTFs from GameObject hierarchies
        var export = new GameObjectExport( exportSettings, logger: logger, materialExport: matExport);

        // Example of gathering GameObjects to be exported (recursively)
        var rootLevelNodes = toExport;

        // Add a scene
        export.AddScene(rootLevelNodes, "My new glTF scene");

        // Async glTF export
        bool success = await export.SaveToFileAndDispose(Savepath);

        if(!success) {
            Debug.LogError("Something went wrong exporting a glTF");
            // Log all exporter messages
            logger.LogAll();
        }
        onComplete.Invoke(success);
    }
}
