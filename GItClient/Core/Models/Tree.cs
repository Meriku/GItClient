using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.PowerShell.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GItClient.Core.Models
{
    public class CommitsTree
    {
        public CommitsTreeNode Head { get; set; }
        public Dictionary<string, CommitsTreeNode> AllNodes { get; set; }

        public CommitsTree()
        {
            AllNodes = new Dictionary<string, CommitsTreeNode>();
        }

        public void Add(GitCommit commit)
        {
            var node = new CommitsTreeNode(commit);
            AllNodes[node.Hash] = node;

            if (Head == null)
            {
                Head = node;
                return;
            }

            foreach(var parentHash in node.Data.ParentHashes)
            {
                var isCommitsParsedCorrectly = AllNodes.ContainsKey(parentHash);

                var parent = isCommitsParsedCorrectly ? AllNodes[parentHash] : AllNodes.Last().Value;
                
                parent.AddChild(node);
            }


        }
    }
    public class CommitsTreeNode
    {
        public string Hash => Data.Hash;
        public GitCommit Data { get; set; }

        public List<CommitsTreeNode> Children { get; set; }
        public List<CommitsTreeNode> Parents { get; set; }

        public CommitsTreeNode(GitCommit data)
        {
            Data = data;
            Children = new List<CommitsTreeNode>();
            Parents = new List<CommitsTreeNode>();
        }

        public void AddChild(CommitsTreeNode node)
        {
            Children.Add(node);
            node.AddParent(this);
        }
        public void AddParent(CommitsTreeNode node)
        {
            Parents.Add(node);
        }
    }
}