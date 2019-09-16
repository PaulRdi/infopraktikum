using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using UnityEditor;
using System.CodeDom.Compiler;
using System.IO;
using Microsoft.CSharp;
using Collectiblox.Model;
namespace Collectiblox.EditorExtensions
{
    //https://stackoverflow.com/questions/3862226/how-to-dynamically-create-a-class
    public static class ClassFileCreator
    {
        /// <summary>
        /// creates a class  from an Attribute format -> this was a "Fingerübung" of reading and adapting questions & documentaiton
        /// </summary>
        /// <param name="format"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static Type CreateClass(AttributeFormat format, Type parent)
        {
            TypeBuilder typeBuilder = GetTypeBuilder(format, parent);
            //ConstructorBuilder constructor = typeBuilder.DefineConstructor(
            //    MethodAttributes.Public |
            //    MethodAttributes.SpecialName |
            //    MethodAttributes.RTSpecialName,
            //    CallingConventions.HasThis,
            //    format.GetTypes());

            ConstructorBuilder constructor = typeBuilder.DefineDefaultConstructor(
                MethodAttributes.Public |
                MethodAttributes.SpecialName |
                MethodAttributes.RTSpecialName);
            foreach (string attributeName in format.nameToType.Keys)
            {
                CreateProperty(typeBuilder, attributeName, Convert.ToType(format.nameToType[attributeName]));
            }

            Type objectType = typeBuilder.CreateType();
            return objectType;
        }

        private static void CreateProperty(TypeBuilder typeBuilder, string propertyName, Type type)
        {
            string fieldName =  char.ToLower(propertyName[0]) + propertyName.Substring(1);
            propertyName =  char.ToUpper(propertyName[0]) + propertyName.Substring(1);
            FieldBuilder fieldBuilder = typeBuilder.DefineField(fieldName, type, FieldAttributes.Private);
            CustomAttributeBuilder attributeBuilder = new CustomAttributeBuilder(
                typeof(SerializeField).GetConstructor(Type.EmptyTypes), new object[] { });
            fieldBuilder.SetCustomAttribute(attributeBuilder);

            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, type, null);

            MethodBuilder getPropertyMethodBuilder = typeBuilder.DefineMethod(
                "get_" + propertyName,
                MethodAttributes.Public |
                MethodAttributes.SpecialName |
                MethodAttributes.HideBySig,
                type,
                Type.EmptyTypes);
            ILGenerator getIl = getPropertyMethodBuilder.GetILGenerator();
            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropertyMethodBuilder);
        }

        private static TypeBuilder GetTypeBuilder(AttributeFormat format, Type parentClass)
        {
            string typeSignature = format.type;
            AssemblyName assemblyName = new AssemblyName(typeSignature);
            System.Reflection.Emit.AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            TypeBuilder typeBuilder = moduleBuilder.DefineType(
                typeSignature,
                TypeAttributes.Public |
                TypeAttributes.Class |
                TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit |
                TypeAttributes.AutoLayout,
                parentClass);
            return typeBuilder;
        }
        //https://forum.unity.com/threads/how-to-create-c-files-as-editor-extension.434606/
        public static void CreateClassFile(AttributeFormat format, Type parent, bool force = false)
        {
            string[] guids = Selection.assetGUIDs;
            if (guids.Length == 0)
                return;

            string classname = char.ToUpper(format.type[0]) + format.type.Substring(1);

            if (Type.GetType(classname) != null && !force)
                throw new System.Exception("type already exists. Cannot create file");

            string template = File.ReadAllText(Application.streamingAssetsPath + "/ClassTemplate.txt");
            string relativePath = "/Scripts/Model/"+classname+".cs";
            template = template.Replace("##CLASSNAME##", format.type);
            string properties = "";
            foreach (string propertyName in format.nameToType.Keys)
            {
                AddPropertyToString(ref properties, propertyName, Convert.ToType(format.nameToType[propertyName]));
            }
            template = template.Replace("##PROPERTIES##", properties);
            template = template.Replace("Int32", "int");
            string errors = "Creating csharp files for card formats had errors: \n\n";
            if (ValidateString(template, ref errors))
            {
                File.WriteAllText(Application.dataPath + relativePath, template);
                AssetDatabase.ImportAsset("Assets"+relativePath);
            }
            else
            {
                throw new System.Exception(errors);
            }
        }
        //https://stackoverflow.com/questions/1361965/compile-simple-string
        private static bool ValidateString(string template, ref string compilerErrors)
        {
            Microsoft.CSharp.CSharpCodeProvider provider = new CSharpCodeProvider();
            ICodeCompiler compiler = provider.CreateCompiler();
            CompilerParameters compilerparams = new CompilerParameters();
            // thank god... https://gamedev.stackexchange.com/questions/130268/how-do-i-compile-a-c-script-at-runtime-and-attach-it-as-a-component-to-a-game-o
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    compilerparams.ReferencedAssemblies.Add(assembly.Location);
                }
                catch (NotSupportedException e)
                {
                    Debug.LogWarning(e.Data.ToString());
                }
            }
            compilerparams.GenerateExecutable = false;
            compilerparams.GenerateInMemory = true;
            CompilerResults results = compiler.CompileAssemblyFromSource(compilerparams, template);
            if (results.Errors.HasErrors)
            {
                StringBuilder errors = new StringBuilder("Compiler Errors :\r\n");
                foreach (CompilerError error in results.Errors)
                {
                    errors.AppendFormat("Line {0},{1}\t: {2}\n", error.Line, error.Column, error.ErrorText);
                }
                compilerErrors += errors.ToString();
                return false;
            }
            else
            {
                return true;
            }
        }

        private static void AddPropertyToString(ref string output, string propertyName, Type type)
        {
            string pn = char.ToLower(propertyName[0]) + propertyName.Substring(1);
            string fn = "_" + pn;
            output += "\t\tpublic " + type.Name + " " + pn + "{\n" +
                "\t\t\tget { return " + fn + ";" + "}\n" + 
                "\t\t}\n";
            output += "\t\t[SerializeField]\n";
            output += "\t\tprivate " + type.Name + " " + fn + ";\n";
        }

        [MenuItem("Cards/Update Card Classes")]
        static void UpdateClasses()
        {
            AttributeFormats formats = AttributeFormats.Load();
            foreach (AttributeFormat format in formats)
            {
                CreateClassFile(format, null, true);
            }

        }
    }
}
