﻿using System;
using System.Collections.Generic;
using System.Linq;
namespace Pure.Data.Gen
{

   
    public class CodeGenEngine: ICodeGenEngine
    {
      
        //private static object oLock = new object();
        //private static bool HasGen = false;

        public string Generate(IDatabase database)
        {
            string outputdir = "";
            //if (HasGen == false)
            //{
            //    lock (oLock)
            //    {
            try
            {
                var config = database.Config;

                ProjectConfig projectConfig = new ProjectConfig();
                projectConfig.Enable = config.EnableCodeGen;
                projectConfig.ClassNameMode = config.CodeGenClassNameMode;
                projectConfig.PropertyNameMode = config.CodeGenPropertyNameMode;
                projectConfig.Name = config.CodeGenProjectName;
                projectConfig.NameSpace = config.CodeGenNameSpace;
                projectConfig.TableFilter = config.CodeGenTableFilter;

                foreach (var temp in config.CodeGenTemplates)
                {
                    GeneraterConfig genConfig = new GeneraterConfig();
                    genConfig.Name = temp.Name;
                    genConfig.OutputFileExtension = temp.OutputFileExtension;
                    genConfig.FilePrefix = temp.FilePrefix;
                    genConfig.FileSuffix = temp.FileSuffix;
                    genConfig.FileNameFormat = temp.FileNameFormat;
                    genConfig.Enabled = temp.Enabled;
                    genConfig.TemplateFileName = temp.TemplateFileName;
                    //genConfig.Template = temp.Template;
                    genConfig.OutputDirectory = temp.OutputDirectory;
                    genConfig.Append = temp.Append;
                    genConfig.Encoding = temp.Encoding;

                    projectConfig.GeneraterConfigs.Add(genConfig);


                }
                var generater = GeneratorHelper.NewGenerator(database);

                generater.ClearCache(projectConfig);
                List<string> filterTables = new List<string>();
                if (config.AutoMigrateOnContainTable != null && config.AutoMigrateOnContainTable != "")
                {
                    filterTables = config.AutoMigrateOnContainTable.ToUpper().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                generater.Run(database, projectConfig, filterTables);

                

                outputdir = DbLoader.GetProjectDirectory(projectConfig);
                if (System.IO.Directory.Exists(outputdir))
                {
                    return outputdir;
                }
                 
            }
            catch (Exception ex)
            {

                throw new Exception("CodeGenEngine发生错误："+ ex, ex.InnerException);
            }

            outputdir = System.IO.Path.Combine(PathHelper.GetBaseDirectory(), "generate");
            return outputdir;

        }


    }
}
