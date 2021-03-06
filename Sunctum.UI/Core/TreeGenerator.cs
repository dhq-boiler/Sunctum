﻿

using NLog;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Sunctum.UI.Core
{
    public static class TreeGenerator
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public static readonly string PATTERN_CANDIDATE = "^(?<indent>\\s*(?<array>-\\s)?)(?<key>[a-zA-Z\\d]+?)(?<hash>:)\\s*?(?<return>[\\|>]?)(?<specify_indent>\\d*)(?<sign>[\\+-]?)$";
        public static readonly string PATTERN_VALUE = "^(?<indent>\\s*(?<array>-\\s)?)(?<key>[a-zA-Z\\d]+?)(?<hash>:)\\s+?(?<value>.+?)$";
        public static readonly string PATTERN_MULTILINE = "^(?<indent>\\s*)(?<part_of_value>.*?)$";

        public static TreeEntry ParseYaml(string yamlString)
        {
            var scr = new StringCollectionReader(yamlString);
            var root = ParseYaml(scr);
            return root;
        }

        private static TreeEntry ParseYaml(StringCollectionReader scr)
        {
            var rootNode = new TreeEntry();
            rootNode.IndentCount = -1;
            rootNode.Children = new List<TreeEntry>();
            string line = null;

            TreeEntry parentNode = rootNode;
            TreeEntry previousNode = rootNode;
            int previousIndentCount = rootNode.IndentCount;

            while ((line = scr.ReadLine()) != null)
            {
                Regex rg_hash = new Regex(PATTERN_CANDIDATE);
                Regex rg_value = new Regex(PATTERN_VALUE);

                if (rg_hash.IsMatch(line) || rg_value.IsMatch(line))
                {
                    Match mc = null;
                    if (rg_hash.IsMatch(line))
                    {
                        mc = rg_hash.Match(line);
                    }
                    else if (rg_value.IsMatch(line))
                    {
                        mc = rg_value.Match(line);
                    }

                    int indent = GetIndentPrefix(mc);
                    bool isHash = IsHash(mc);
                    bool isArray = IsArray(mc);
                    bool saveReturn = WillSaveReturn(mc);
                    bool replaceReturnToHalfSpace = WillReplaceReturnToHalfSpace(mc);
                    int? indentSpecified = GetIndentSpecified(mc);
                    bool saveLastReturn = WillSaveLastReturn(mc);
                    bool removeLastReturn = WillRemoveLastReturn(mc);

                    if (previousIndentCount < indent) //descending
                    {
                        parentNode = previousNode;

                        if (isArray)
                        {
                            var arrayNode = new TreeEntry(parentNode, $"[{parentNode.Children.Count}]", indent, true);
                            parentNode.Children.Add(arrayNode);
                            parentNode = arrayNode;
                        }

                        previousNode = ProcessCurrentLine(scr, line, parentNode, mc, indent, isHash, isArray, saveReturn, replaceReturnToHalfSpace, saveLastReturn, removeLastReturn);
                    }
                    else if (previousIndentCount == indent) //loop
                    {
                        previousNode = ProcessCurrentLine(scr, line, parentNode, mc, indent, isHash, isArray, saveReturn, replaceReturnToHalfSpace, saveLastReturn, removeLastReturn);
                    }
                    else if (parentNode.IndentCount == indent) //ascending
                    {
                        parentNode = parentNode.Parent;

                        previousNode = ProcessCurrentLine(scr, line, parentNode, mc, indent, isHash, isArray, saveReturn, replaceReturnToHalfSpace, saveLastReturn, removeLastReturn);
                    }
                    else
                    {
                        while (parentNode.IndentCount > indent)
                        {
                            parentNode = parentNode.Parent;
                        }
                        parentNode = parentNode.Parent;

                        previousNode = ProcessCurrentLine(scr, line, parentNode, mc, indent, isHash, isArray, saveReturn, replaceReturnToHalfSpace, saveLastReturn, removeLastReturn);
                    }

                    previousIndentCount = indent;
                }
            }

            return rootNode;
        }

        private static bool WillRemoveLastReturn(Match mc)
        {
            return mc.Groups["sign"].Value == "-";
        }

        private static bool WillSaveLastReturn(Match mc)
        {
            return mc.Groups["sign"].Value == "+";
        }

        private static int? GetIndentSpecified(Match mc)
        {
            return !string.IsNullOrEmpty(mc.Groups["specify_indent"].Value) ? (int?)int.Parse(mc.Groups["specify_indent"].Value) : null;
        }

        private static bool WillReplaceReturnToHalfSpace(Match mc)
        {
            return mc.Groups["return"].Value.StartsWith(">");
        }

        private static bool WillSaveReturn(Match mc)
        {
            return mc.Groups["return"].Value.StartsWith("|");
        }

        private static bool IsArray(Match mc)
        {
            return mc.Groups["array"].Value == "- ";
        }

        private static bool IsHash(Match mc)
        {
            return mc.Groups["hash"].Value == ":";
        }

        private static int GetIndentPrefix(Match mc)
        {
            return mc.Groups["indent"].Value.Length;
        }

        private static TreeEntry ProcessCurrentLine(StringCollectionReader scr, string line, TreeEntry parentNode, Match mc, int indent, bool isHash, bool isArray, bool saveReturn, bool replaceReturnToHalfSpace, bool saveLastReturn, bool removeLastReturn)
        {
            var currentNode = new TreeEntry(parentNode, mc.Groups["key"].Value, indent, isArray);

            parentNode.Children.Add(currentNode);

            if (isHash)
            {
                ProcessHash(scr, line, parentNode, mc, saveReturn, replaceReturnToHalfSpace, saveLastReturn, removeLastReturn, currentNode);
            }

            return currentNode;
        }

        private static TreeEntry ProcessHash(StringCollectionReader scr, string line, TreeEntry parentNode, Match mc, bool saveReturn, bool replaceReturnToHalfSpace, bool saveLastReturn, bool removeLastReturn, TreeEntry currentNode)
        {
            if (saveReturn || replaceReturnToHalfSpace)
            {
                string multilineValue = "";
                while ((line = scr.ReadLine()) != null)
                {
                    var rg_hash = new Regex(PATTERN_VALUE);
                    if (rg_hash.IsMatch(line) || line == null)
                    {
                        //複数行の文字列の終わり
                        multilineValue = ProcessReturn(saveReturn, replaceReturnToHalfSpace, removeLastReturn, multilineValue);

                        currentNode.Value = multilineValue;

                        mc = rg_hash.Match(line);
                        int indent = GetIndentPrefix(mc);
                        bool isHash = IsHash(mc);
                        bool isArray = IsArray(mc);
                        saveReturn = WillSaveReturn(mc);
                        replaceReturnToHalfSpace = WillReplaceReturnToHalfSpace(mc);
                        int? indentSpecified = GetIndentSpecified(mc);
                        saveLastReturn = WillSaveLastReturn(mc);
                        removeLastReturn = WillRemoveLastReturn(mc);

                        //再帰処理
                        return ProcessCurrentLine(scr, line, parentNode, mc, indent, isHash, isArray, saveReturn, replaceReturnToHalfSpace, saveLastReturn, removeLastReturn);
                    }
                    else
                    {
                        multilineValue += line;

                        multilineValue += Environment.NewLine;

                        multilineValue = ProcessReturn(saveReturn, replaceReturnToHalfSpace, false, multilineValue);
                    }
                }
            }
            else
            {
                var rg = new Regex(PATTERN_VALUE);
                if (rg.IsMatch(line))
                {
                    mc = rg.Match(line);
                    currentNode.Value = mc.Groups["value"].Value;
                }
            }

            return currentNode;
        }

        private static string ProcessReturn(bool saveReturn, bool replaceReturnToHalfSpace, bool removeLastReturn, string multilineValue)
        {
            if (replaceReturnToHalfSpace)
            {
                multilineValue = multilineValue.Replace(Environment.NewLine, " ");
            }

            if (removeLastReturn)
            {
                if (multilineValue.EndsWith(Environment.NewLine))
                {
                    multilineValue = multilineValue.Remove(multilineValue.LastIndexOf(Environment.NewLine));
                }
            }

            return multilineValue;
        }
    }
}
