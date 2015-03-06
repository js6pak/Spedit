﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spedit.SPCondenser;
using System.IO;
using System.Xml;
using System.Windows;

namespace Spedit.Interop
{
    public static class ConfigLoader
    {
        public static Config[] Load()
        {
            List<Config> configs = new List<Config>();
            if (File.Exists("sourcepawn\\configs\\Configs.xml"))
            {
                try
                {
                    XmlDocument document = new XmlDocument();
                    document.Load("sourcepawn\\configs\\Configs.xml");
                    if (document.ChildNodes.Count < 1)
                    {
                        throw new Exception("No main 'Configurations' node.");
                    }
                    XmlNode mainNode = document.ChildNodes[0];
                    if (mainNode.ChildNodes.Count < 1)
                    {
                        throw new Exception("No 'config' nodes found.");
                    }
                    for (int i = 0; i < mainNode.ChildNodes.Count; ++i)
                    {
                        XmlNode node = mainNode.ChildNodes[i];
                        string _Name = node.Attributes["Name"].Value;
                        string _SMDirectory = node.Attributes["SMDirectory"].Value;
                        string _Standard = node.Attributes["Standard"].Value;
                        bool IsStandardConfig = false;
                        if (_Standard != "0" && !string.IsNullOrWhiteSpace(_Standard))
                        {
                            IsStandardConfig = true;
                        }
                        string _CopyDirectory = node.Attributes["CopyDirectory"].Value;
                        string _ServerFile = node.Attributes["ServerFile"].Value;
                        string _ServerArgs = node.Attributes["ServerArgs"].Value;
                        string _PostCmd = node.Attributes["PostCmd"].Value;
                        string _PreCmd = node.Attributes["PreCmd"].Value;
                        int _OptimizationLevel = 2, _VerboseLevel = 1;
                        int subValue;
                        if (int.TryParse(node.Attributes["OptimizationLevel"].Value, out subValue))
                        {
                            _OptimizationLevel = subValue;
                        }
                        if (int.TryParse(node.Attributes["VerboseLevel"].Value, out subValue))
                        {
                            _VerboseLevel = subValue;
                        }
                        Config c = new Config() { Name = _Name, SMDirectory = _SMDirectory, Standard = IsStandardConfig
                            , CopyDirectory = _CopyDirectory, ServerFile = _ServerFile, ServerArgs = _ServerArgs
                            , PostCmd = _PostCmd, PreCmd = _PreCmd, OptimizeLevel = _OptimizationLevel, VerboseLevel = _VerboseLevel};
                        if (IsStandardConfig)
                        {
                            c.LoadSMDef();
                        }
                        configs.Add(c);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("An error appeared while reading the configs. Without them, the editor wont start. Reinstall program!" + Environment.NewLine + "Details: " + e.Message
                        , "Error while reading configs."
                        , MessageBoxButton.OK
                        , MessageBoxImage.Warning);
                    Environment.Exit(Environment.ExitCode);
                }
            }
            else
            {
                MessageBox.Show("The Editor could not find the Configs.xml file. Without it, the editor wont start. Reinstall program.", "File not found.", MessageBoxButton.OK, MessageBoxImage.Warning);
                Environment.Exit(Environment.ExitCode);
            }
            return configs.ToArray();
        }
    }

    public class Config
    {
        public string Name = string.Empty;

        public bool Standard = false;

        public string SMDirectory = string.Empty;
        public string CopyDirectory = string.Empty;
        public string ServerFile = string.Empty;
        public string ServerArgs = string.Empty;

        public string PostCmd = string.Empty;
        public string PreCmd = string.Empty;

        public int OptimizeLevel = 2;
        public int VerboseLevel = 1;

        private CondensedSourcepawnDefinition SMDef;

        public CondensedSourcepawnDefinition GetSMDef()
        {
            if (SMDef == null)
            {
                LoadSMDef();
            }
            return SMDef;
        }

        public void InvalidateSMDef()
        {
            this.SMDef = null;
        }

        public void LoadSMDef()
        {
            if (this.SMDef != null)
            {
                return;
            }
            try
            {
                this.SMDef = SourcepawnCondenser.Condense(SMDirectory);
            }
            catch (Exception)
            {
                this.SMDef = new CondensedSourcepawnDefinition(); //this could be dangerous...
            }
        }
    }
}