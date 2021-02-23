using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet.Emit;
using dnlib.DotNet;
using System.IO;

namespace IL2CPP_Offsets_Dumper
{
    class Program
    {
        public static ModuleDefMD BaseProgram;
        static void Main(string[] args)
        {
            //string filename = @"C:\il2cppdmp\DummyDll\Rust.Data.dll";
            //BaseProgram = ModuleDefMD.Load(filename);
            int i = 0;
            string types = "";
            string result = "namespace Offsets{\n";

            List<string> declared = new List<string>();
            foreach (var fileName in Directory.GetFiles(@"C:\il2cppdmp\DummyDll"))
                {
                if (!fileName.Contains("Assembly-CSharp.dll"))
                    continue;
                BaseProgram = ModuleDefMD.Load(fileName);
                foreach (var type in BaseProgram.Types)
                {
                    if (!type.Name.Contains("<") && type.Fields.Count > 0)
                    {
                        i++;
                        string typeName = "";
                        if (type.Name.String.Split('.').Length > 0)
                            typeName = type.Name.String.Split('.')[type.Name.String.Split('.').Length - 1];
                        

                        string file = "";
                        file += "namespace " + typeName.Replace("`", "").Replace("<", "").Replace(">","") + "\n{\n";
                        foreach (var f in type.Fields)
                        {                            
                            if (f.CustomAttributes.Where(x => x.TypeFullName.Contains("FieldOffsetAttribute")).Count() > 0)
                            {
                                foreach (var attr in f.CustomAttributes)
                                {
                                    if (attr.TypeFullName.Contains("FieldOffsetAttribute"))
                                    {
                                        string fieldTypeName = "";
                                        if (f.FieldType.TypeName.Split('.').Length > 0)
                                            fieldTypeName = f.FieldType.TypeName.Split('.')[f.FieldType.TypeName.Split('.').Length - 1];
                                        else typeName = f.DeclaringType.Name;

                                        fieldTypeName = fieldTypeName.Replace("Boolean", "bool").Replace("UInt32", "uint32_t").Replace("Int32", "int32_t").Replace("String", "string").Replace("Byte", "char");

                                        if (fieldTypeName.Contains("`") || fieldTypeName.Contains("["))
                                            continue;

                                        string fieldName = "";
                                        if (f.Name.String.Split('.').Length > 0)
                                            fieldName = f.Name.String.Split('.')[f.Name.String.Split('.').Length - 1];
                                        else fieldName = f.Name;

                                        if (!declared.Contains(fieldTypeName + "__" + fieldName.Replace("<", "").Replace(">", "")))
                                        {
                                            file += "constexpr auto " + fieldName.Replace("<", "").Replace(">", "") + " = " + attr.Fields.First().Value + "; // " + fieldTypeName + "::" + fieldName + "\n";
                                            declared.Add(fieldTypeName + "__" + fieldName.Replace("<", "").Replace(">", ""));
                                        }
                                    }
                                }
                                //Console.WriteLine(f.Name + " : " + ((int)(f.CustomAttributes.Where(x => .RawData[4])).ToString());
                            }
                        }
                        //file += "   };\n";
                        file += "};";
                        result += file + "\n";
                    }
                }
                result += "\n}";
                result = types + "\n" + result + "\n}";
                Console.WriteLine(result);
            }
        }
    }
}
