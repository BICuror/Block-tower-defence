using UnityEditor;
using System.IO;
using System;

namespace CuroCodeGen
{
    public static class ScriptFileGenerator
    {
        public static bool Generate(Type generatorType)
        {
            var changed = false;
            
            var generator = (ICodeGenerator)Activator.CreateInstance(generatorType); 
            var context = new GeneratorContext(); 
            generator.Execute(context);
            
            if (GenerateScriptFromContext(context)) 
            { 
                changed = true;
            }

            if (changed)
            {
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
            }
            
            return changed;
        }

        private static bool GenerateScriptFromContext(GeneratorContext context)
        {
            var changed = false;

            var folderPath = context.FolderPath;

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            foreach (var code in context.codeList)
            {
                var hierarchy = code.FileName.Split('/');
                var path = folderPath;
                for (int i = 0; i < hierarchy.Length; i++)
                {
                    path += "/" + hierarchy[i];
                    if (i == hierarchy.Length - 1) break;
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                }

                if (File.Exists(path))
                {
                    var text = File.ReadAllText(path);
                    if (text == code.Text) continue;
                }

                File.WriteAllText(path, code.Text);
                changed = true;
            }
            
            return changed;
        }
    }
}