/*! 
@file JumpPointFinder.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eppathfinding.cs>
@date July 16, 2013
@brief Jump Point Search Algorithm Interface
@version 2.0

@section LICENSE

The MIT License (MIT)

Copyright (c) 2013 Woong Gyu La <juhgiyo@gmail.com>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.

@section DESCRIPTION

An Interface for the Jump Point Search Algorithm Class.

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;

namespace EpPathFinding
{

    public delegate float HeuristicDelegate(int iDx, int iDy);

    public class JumpPointParam 
    {

        public JumpPointParam(BaseGrid iGrid, GridPos iStartPos, GridPos iEndPos, bool iAllowEndNodeUnWalkable = true, bool iCrossCorner = true, bool iCrossAdjacentPoint = true, HeuristicMode iMode = HeuristicMode.EUCLIDEAN)
        {
            switch (iMode)
            {
                case HeuristicMode.MANHATTAN:
                    m_heuristic = new HeuristicDelegate(Heuristic.Manhattan);
                    break;
                case HeuristicMode.EUCLIDEAN:
                    m_heuristic = new HeuristicDelegate(Heuristic.Euclidean);
                    break;
                case HeuristicMode.CHEBYSHEV:
                    m_heuristic = new HeuristicDelegate(Heuristic.Chebyshev);
                    break;
                default:
                    m_heuristic = new HeuristicDelegate(Heuristic.Euclidean);
                    break;
            }
            m_allowEndNodeUnWalkable = iAllowEndNodeUnWalkable;
            m_crossAdjacentPoint = iCrossAdjacentPoint;
            m_crossCorner = iCrossCorner;
            openList = new List<Node>();
            copyList= new List<Node>();
            listParam = new List<Node>();
            cCList = new List<Node>();
            m_searchGrid = iGrid;
            m_startNode = m_searchGrid.GetNodeAt(iStartPos.x, iStartPos.y);
            m_endNode = m_searchGrid.GetNodeAt(iEndPos.x, iEndPos.y);
            if (m_startNode == null)
                m_startNode = new Node(iStartPos.x, iStartPos.y, true);
            if (m_endNode == null)
                m_endNode = new Node(iEndPos.x, iEndPos.y, true);
            m_useRecursive = false;
        }

        public JumpPointParam(BaseGrid iGrid, bool iAllowEndNodeUnWalkable = true, bool iCrossCorner = true, bool iCrossAdjacentPoint=true, HeuristicMode iMode = HeuristicMode.EUCLIDEAN)
        {
            switch (iMode)
            {
                case HeuristicMode.MANHATTAN:
                    m_heuristic = new HeuristicDelegate(Heuristic.Manhattan);
                    break;
                case HeuristicMode.EUCLIDEAN:
                    m_heuristic = new HeuristicDelegate(Heuristic.Euclidean);
                    break;
                case HeuristicMode.CHEBYSHEV:
                    m_heuristic = new HeuristicDelegate(Heuristic.Chebyshev);
                    break;
                default:
                    m_heuristic = new HeuristicDelegate(Heuristic.Euclidean);
                    break;
            }
            m_allowEndNodeUnWalkable = iAllowEndNodeUnWalkable;
            m_crossAdjacentPoint = iCrossAdjacentPoint;
            m_crossCorner = iCrossCorner;

            openList = new List<Node>();
            copyList = new List<Node>();
            cCList = new List<Node>();
            m_searchGrid = iGrid;
            m_startNode = null;
            m_endNode = null;
            m_useRecursive = false;
        }

        public void SetHeuristic(HeuristicMode iMode)
        {
            m_heuristic = null;
            switch (iMode)
            {
                case HeuristicMode.MANHATTAN:
                    m_heuristic = new HeuristicDelegate(Heuristic.Manhattan);
                    break;
                case HeuristicMode.EUCLIDEAN:
                    m_heuristic = new HeuristicDelegate(Heuristic.Euclidean);
                    break;
                case HeuristicMode.CHEBYSHEV:
                    m_heuristic = new HeuristicDelegate(Heuristic.Chebyshev);
                    break;
                default:
                    m_heuristic = new HeuristicDelegate(Heuristic.Euclidean);
                    break;
            }
        }

        public void Reset(GridPos iStartPos, GridPos iEndPos, BaseGrid iSearchGrid = null)
        {
            openList.Clear();
            m_startNode = null;
            m_endNode = null;

            if (iSearchGrid != null)
                m_searchGrid = iSearchGrid;
            m_searchGrid.Reset();
            m_startNode = m_searchGrid.GetNodeAt(iStartPos.x, iStartPos.y);
            m_endNode = m_searchGrid.GetNodeAt(iEndPos.x, iEndPos.y);
            if (m_startNode == null)
                m_startNode = new Node(iStartPos.x, iStartPos.y, true);
            if (m_endNode == null)
                m_endNode = new Node(iEndPos.x, iEndPos.y, true);


        }

        public object Clone()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                if (this.GetType().IsSerializable)
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, this);
                    stream.Position = 0;
                    return formatter.Deserialize(stream);
                }
                return null;
            }
        }

        public bool CrossAdjacentPoint
        {
            get
            {
                return m_crossCorner && m_crossAdjacentPoint;
            }
            set
            {
                m_crossAdjacentPoint = value;
            }
        }

        public bool CrossCorner
        {
            get
            {
                return m_crossCorner;
            }
            set
            {
                m_crossCorner = value;
            }
        }

        public bool AllowEndNodeUnWalkable
        {
            get
            {
                return m_allowEndNodeUnWalkable;
            }
            set
            {
                m_allowEndNodeUnWalkable = value;
            }
        }

        public HeuristicDelegate HeuristicFunc
        {
            get
            {
                return m_heuristic;
            }
        }

        public BaseGrid SearchGrid
        {
            get
            {
                return m_searchGrid;
            }
            set
            {
                m_searchGrid = value;
            }
        }

        public Node StartNode
        {
            get
            {
                return m_startNode;
            }
        }
        public Node EndNode
        {
            get
            {
                return m_endNode;
            }
        }

        public bool UseRecursive
        {
            get
            {
                return m_useRecursive;
            }
            set
            {
                m_useRecursive = value;
            }
        }
        protected HeuristicDelegate m_heuristic;
        protected bool m_crossAdjacentPoint;
        protected bool m_crossCorner;
        protected bool m_allowEndNodeUnWalkable;

        protected bool m_useRecursive;

        public BaseGrid m_searchGrid,cloneSearchGrid;
        protected Node m_startNode;
        protected Node m_endNode;

        public List<Node> openList;
        public List<Node> copyList;
        public List<Node> cCList;
        public List<Node> listParam;
    }
    [Serializable]
    class JumpPointFinder
    {

        public int numExp = 0;
        public bool hitBorder = false;
        bool pathUsedBorder = false;
        List<Node> CopiedOpenList;
        bool Copying = true;
        public  JumpPointParam CloneParam;

         public List<Node> CopiedOpenListOut
        {
            get
            {

                return CopiedOpenListOut= null;
            }
            set
            {
                this.CopiedOpenList = null;
            }
        }
        public void getParam(JumpPointParam toRetrun)
        {
            JumpPointParam param = new JumpPointParam(toRetrun.SearchGrid, true, false, false, HeuristicMode.EUCLIDEAN);
            param.CrossCorner = false;
            param.CrossAdjacentPoint = false;
            param.UseRecursive = false;
            //param.Reset(startPos, endPos);
            for (int i = 0; i < toRetrun.m_searchGrid.width; i++)
                for (int j = 0; j < toRetrun.m_searchGrid.height; j++)
                {
                    GridPos Pnode = new GridPos(i, j);
                    Node JNode, CNode;
                    JNode = param.SearchGrid.GetNodeAt(Pnode);
                    CNode = toRetrun.SearchGrid.GetNodeAt(Pnode);
                    try
                    {
                        if (CNode.startToCurNodeLen>0)
                        {

                            JNode.isClosed = CNode.isClosed;
                            JNode.isOpened = CNode.isOpened;
                            JNode.parent = CNode.parent;
                            JNode.heuristicCurNodeToEndLen = CNode.heuristicCurNodeToEndLen;
                            JNode.heuristicStartToEndLen = CNode.heuristicStartToEndLen;
                            if (CNode.startToCurNodeLen > 0)
                                Console.WriteLine("Finally there is sth here");
                            JNode.startToCurNodeLen = CNode.startToCurNodeLen;
                        }
                    }
                    catch
                    {
                        //Console.WriteLine("Skipping");

                    }


                }


            foreach (Node node in toRetrun.openList)
            {
                param.openList.Add(node);
            }
            CloneParam = param;
        }

        public  List<GridPos> FindPath(JumpPointParam iParam, int [,] borders,int numberCuts, List<Node> CopiedOpenListIn )
        {

            List<Node> tOpenList;
            tOpenList = iParam.openList;

            if (numberCuts >1)
         
            {
                tOpenList = new List<Node>();
                tOpenList = iParam.openList;

                foreach (Node node in iParam.copyList)
                {

                    tOpenList.Add(node);
                }
                iParam.copyList.Clear();
                iParam.cCList.Clear();
            }
                

            Node tStartNode = iParam.StartNode;
            Node tEndNode = iParam.EndNode;
            Node tNode;
            
            

            bool revertEndNodeWalkable = false;
            // push the start node into the open list
            if (numberCuts == 1)
            {
                tStartNode.startToCurNodeLen = 0;
                tStartNode.heuristicStartToEndLen = 0;
                tOpenList.Add(tStartNode);
                tStartNode.isOpened = true;
                numExp += 1;
            }

            if (iParam.AllowEndNodeUnWalkable && !iParam.SearchGrid.IsWalkableAt(tEndNode.x, tEndNode.y))
            {
                iParam.SearchGrid.SetWalkableAt(tEndNode.x, tEndNode.y, true);
                revertEndNodeWalkable = true;
            }

            // while the open list is not empty
            while (tOpenList.Count > 0)
            {
                numExp += 1;
                // pop the position of node which has the minimum `f` value.
                tOpenList.Sort();
                tNode = (Node)tOpenList[0];
                tOpenList.RemoveAt(0);

                tNode.isClosed = true;

                if (borders[tNode.x, tNode.y] > 0 & Copying)
                {
                    pathUsedBorder = true;
                    foreach (Node node in tOpenList)
                    {
                        if (borders[node.x, node.y] == 0)
                        {
                            Node CNode = new Node(node.x, node.y, true);
                            CNode.isClosed = node.isClosed;
                            CNode.isOpened = node.isOpened;
                            CNode.parent = node.parent;
                            CNode.heuristicCurNodeToEndLen = node.heuristicCurNodeToEndLen;
                            CNode.heuristicStartToEndLen = node.heuristicStartToEndLen;
                            CNode.startToCurNodeLen = node.startToCurNodeLen;
                            iParam.copyList.Add(CNode);
                        }


                    }
                    Copying = false;

                }


                Node JNode = new Node(tNode.x, tNode.y);
                GridPos Pnode = new GridPos(tNode.x, tNode.y);

                if (!pathUsedBorder)
                {

                    //JNode = iParam.SearchGrid.GetNodeAt(Pnode);
                    if (borders[tNode.x, tNode.y] == 0)
                    {
                        JNode.isClosed = true;
                        JNode.isOpened = tNode.isOpened;
                        JNode.parent = tNode.parent;
                        JNode.heuristicCurNodeToEndLen = tNode.heuristicCurNodeToEndLen;
                        JNode.heuristicStartToEndLen = tNode.heuristicStartToEndLen;
                        JNode.startToCurNodeLen = tNode.startToCurNodeLen;
                        iParam.cCList.Add(JNode);

                    }
                }




                if (tNode.Equals(tEndNode))
                {
                    if (revertEndNodeWalkable)
                    {
                        iParam.SearchGrid.SetWalkableAt(tEndNode.x, tEndNode.y, false);
                    }
                    return Node.Backtrace(tNode);// rebuilding path
                }

                identifySuccessors(iParam, tNode);
            }

            if (revertEndNodeWalkable)
            {
                iParam.SearchGrid.SetWalkableAt(tEndNode.x, tEndNode.y, false);
            }


            // fail to find the path
            return new List<GridPos>();
        }


        private static void identifySuccessors(JumpPointParam iParam, Node iNode)
        {
            HeuristicDelegate tHeuristic = iParam.HeuristicFunc;
            List<Node> tOpenList = iParam.openList;
            int tEndX = iParam.EndNode.x;
            int tEndY = iParam.EndNode.y;
            GridPos tNeighbor;
            GridPos? tJumpPoint;
            Node tJumpNode;

            List<GridPos> tNeighbors = findNeighbors(iParam, iNode);
            for (int i = 0; i < tNeighbors.Count; i++)
            {
                tNeighbor = tNeighbors[i];
                if(iParam.UseRecursive)
                    tJumpPoint = jump(iParam, tNeighbor.x, tNeighbor.y, iNode.x, iNode.y);
                else
                    tJumpPoint = jumpLoop(iParam, tNeighbor.x, tNeighbor.y, iNode.x, iNode.y);
                if (tJumpPoint != null)
                {
                    tJumpNode = iParam.SearchGrid.GetNodeAt(tJumpPoint.Value.x, tJumpPoint.Value.y);
                    if (tJumpNode == null)
                    {
                        if (iParam.EndNode.x == tJumpPoint.Value.x && iParam.EndNode.y == tJumpPoint.Value.y)
                            tJumpNode = iParam.SearchGrid.GetNodeAt(tJumpPoint.Value);
                    }
                    if (tJumpNode.isClosed)
                    {
                        continue;
                    }
                    // include distance, as parent may not be immediately adjacent:
                    float tCurNodeToJumpNodeLen = tHeuristic(Math.Abs(tJumpPoint.Value.x - iNode.x), Math.Abs(tJumpPoint.Value.y - iNode.y));
                    float tStartToJumpNodeLen = iNode.startToCurNodeLen + tCurNodeToJumpNodeLen; // next `startToCurNodeLen` value

                    if (!tJumpNode.isOpened || tStartToJumpNodeLen < tJumpNode.startToCurNodeLen)
                    {
                        tJumpNode.startToCurNodeLen = tStartToJumpNodeLen;
                        tJumpNode.heuristicCurNodeToEndLen = (tJumpNode.heuristicCurNodeToEndLen == null ? tHeuristic(Math.Abs(tJumpPoint.Value.x - tEndX), Math.Abs(tJumpPoint.Value.y - tEndY)) : tJumpNode.heuristicCurNodeToEndLen);
                        tJumpNode.heuristicStartToEndLen = tJumpNode.startToCurNodeLen + tJumpNode.heuristicCurNodeToEndLen.Value;
                        tJumpNode.parent = iNode;

                        if (!tJumpNode.isOpened)
                        {
                            tOpenList.Add(tJumpNode);
                            tJumpNode.isOpened = true;
                        }
                    }
                }
            }
        }

        private class JumpSnapshot
        {
            public int iX;
            public int iY;
            public int iPx;
            public int iPy;
            public int tDx;
            public int tDy;
            public GridPos? jx;
            public GridPos? jy;
            public int stage;
            public JumpSnapshot()
            {
            
                iX=0;
                iY=0;
                iPx=0;
                iPy=0;
                tDx=0;
                tDy=0;
                jx=null;
                jy=null;
                stage=0;
            }
        }

        private static GridPos? jumpLoop(JumpPointParam iParam, int iX, int iY, int iPx, int iPy)
        {
            GridPos? retVal = null;
            Stack<JumpSnapshot> stack = new Stack<JumpSnapshot>();

            JumpSnapshot currentSnapshot = new JumpSnapshot();
            JumpSnapshot newSnapshot = null;
            currentSnapshot.iX = iX;
            currentSnapshot.iY = iY;
            currentSnapshot.iPx = iPx;
            currentSnapshot.iPy = iPy;
            currentSnapshot.stage = 0;
            
            stack.Push(currentSnapshot);
            while (stack.Count != 0)
            {
                currentSnapshot = stack.Pop();
                switch (currentSnapshot.stage)
                {
                    case 0:
                        if (!iParam.SearchGrid.IsWalkableAt(currentSnapshot.iX, currentSnapshot.iY))
                        {
                            retVal = null;
                            continue;
                        }
                        else if (iParam.SearchGrid.GetNodeAt(currentSnapshot.iX, currentSnapshot.iY).Equals(iParam.EndNode))
                        {
                            retVal = new GridPos(currentSnapshot.iX, currentSnapshot.iY);
                            continue;
                        }

                        currentSnapshot.tDx = currentSnapshot.iX - currentSnapshot.iPx;
                        currentSnapshot.tDy = currentSnapshot.iY - currentSnapshot.iPy;
                        currentSnapshot.jx = null;
                        currentSnapshot.jy = null;
                        if (iParam.CrossCorner)
                        {
                            // check for forced neighbors
                            // along the diagonal
                            if (currentSnapshot.tDx != 0 && currentSnapshot.tDy != 0)
                            {
                                if ((iParam.SearchGrid.IsWalkableAt(currentSnapshot.iX - currentSnapshot.tDx, currentSnapshot.iY + currentSnapshot.tDy) && !iParam.SearchGrid.IsWalkableAt(currentSnapshot.iX - currentSnapshot.tDx, currentSnapshot.iY)) ||
                                    (iParam.SearchGrid.IsWalkableAt(currentSnapshot.iX + currentSnapshot.tDx, currentSnapshot.iY - currentSnapshot.tDy) && !iParam.SearchGrid.IsWalkableAt(currentSnapshot.iX, currentSnapshot.iY - currentSnapshot.tDy)))
                                {
                                    retVal= new GridPos(currentSnapshot.iX, currentSnapshot.iY);
                                    continue;
                                }
                            }
                            // horizontally/vertically
                            else
                            {
                                if (currentSnapshot.tDx != 0)
                                { 
                                    // moving along x
                                    if ((iParam.SearchGrid.IsWalkableAt(currentSnapshot.iX + currentSnapshot.tDx, currentSnapshot.iY + 1) && !iParam.SearchGrid.IsWalkableAt(currentSnapshot.iX, currentSnapshot.iY + 1)) ||
                                        (iParam.SearchGrid.IsWalkableAt(currentSnapshot.iX + currentSnapshot.tDx, currentSnapshot.iY - 1) && !iParam.SearchGrid.IsWalkableAt(currentSnapshot.iX, currentSnapshot.iY - 1)))
                                    {
                                        retVal= new GridPos(currentSnapshot.iX, currentSnapshot.iY);
                                        continue;
                                    }
                                }
                                else
                                {
                                    if ((iParam.SearchGrid.IsWalkableAt(currentSnapshot.iX + 1, currentSnapshot.iY + currentSnapshot.tDy) && !iParam.SearchGrid.IsWalkableAt(currentSnapshot.iX + 1, currentSnapshot.iY)) ||
                                        (iParam.SearchGrid.IsWalkableAt(currentSnapshot.iX - 1, currentSnapshot.iY + currentSnapshot.tDy) && !iParam.SearchGrid.IsWalkableAt(currentSnapshot.iX - 1, currentSnapshot.iY)))
                                    {
                                        retVal= new GridPos(currentSnapshot.iX, currentSnapshot.iY);
                                        continue;
                                    }
                                }
                            }
                            // when moving diagonally, must check for vertical/horizontal jump points
                            if (currentSnapshot.tDx != 0 && currentSnapshot.tDy != 0)
                            {
                                currentSnapshot.stage = 1;
                                stack.Push(currentSnapshot);

                                newSnapshot = new JumpSnapshot();
                                newSnapshot.iX = currentSnapshot.iX + currentSnapshot.tDx;
                                newSnapshot.iY = currentSnapshot.iY;
                                newSnapshot.iPx = currentSnapshot.iX;
                                newSnapshot.iPy = currentSnapshot.iY;
                                newSnapshot.stage = 0;
                                stack.Push(newSnapshot);
                                continue;
                            }

                            // moving diagonally, must make sure one of the vertical/horizontal
                            // neighbors is open to allow the path

                            // moving diagonally, must make sure one of the vertical/horizontal
                            // neighbors is open to allow the path
                            if (iParam.SearchGrid.IsWalkableAt(currentSnapshot.iX + currentSnapshot.tDx, currentSnapshot.iY) || iParam.SearchGrid.IsWalkableAt(currentSnapshot.iX, currentSnapshot.iY + currentSnapshot.tDy))
                            {
                                newSnapshot = new JumpSnapshot();
                                newSnapshot.iX = currentSnapshot.iX + currentSnapshot.tDx;
                                newSnapshot.iY = currentSnapshot.iY + currentSnapshot.tDy;
                                newSnapshot.iPx = currentSnapshot.iX;
                                newSnapshot.iPy = currentSnapshot.iY;
                                newSnapshot.stage = 0;
                                stack.Push(newSnapshot);
                                continue;
                            }
                            else if (iParam.CrossAdjacentPoint)
                            {
                                newSnapshot = new JumpSnapshot();
                                newSnapshot.iX = currentSnapshot.iX + currentSnapshot.tDx;
                                newSnapshot.iY = currentSnapshot.iY + currentSnapshot.tDy;
                                newSnapshot.iPx = currentSnapshot.iX;
                                newSnapshot.iPy = currentSnapshot.iY;
                                newSnapshot.stage = 0;
                                stack.Push(newSnapshot);
                                continue;
                            }
                        }
                        else //if (!iParam.CrossCorner)
                        {
                            // check for forced neighbors
                            // along the diagonal
                            if (currentSnapshot.tDx != 0 && currentSnapshot.tDy != 0)
                            {
                                if ((iParam.SearchGrid.IsWalkableAt(currentSnapshot.iX + currentSnapshot.tDx, currentSnapshot.iY + currentSnapshot.tDy) && iParam.SearchGrid.IsWalkableAt(currentSnapshot.iX, currentSnapshot.iY + currentSnapshot.tDy) && !iParam.SearchGrid.IsWalkableAt(currentSnapshot.iX + currentSnapshot.tDx, currentSnapshot.iY)) ||
                                    (iParam.SearchGrid.IsWalkableAt(currentSnapshot.iX + currentSnapshot.tDx, currentSnapshot.iY + currentSnapshot.tDy) && iParam.SearchGrid.IsWalkableAt(currentSnapshot.iX + currentSnapshot.tDx, currentSnapshot.iY) && !iParam.SearchGrid.IsWalkableAt(currentSnapshot.iX, currentSnapshot.iY + currentSnapshot.tDy)))
                                {
                                    retVal = new GridPos(currentSnapshot.iX, currentSnapshot.iY);
                                    continue;
                                }
                            }
                            // horizontally/vertically
                            else
                            {
                                if (currentSnapshot.tDx != 0)
                                { 
                                    // moving along x
                                    if ((iParam.SearchGrid.IsWalkableAt(currentSnapshot.iX, currentSnapshot.iY + 1) && !iParam.SearchGrid.IsWalkableAt(currentSnapshot.iX - currentSnapshot.tDx, currentSnapshot.iY + 1)) ||
                                        (iParam.SearchGrid.IsWalkableAt(currentSnapshot.iX, currentSnapshot.iY - 1) && !iParam.SearchGrid.IsWalkableAt(currentSnapshot.iX - currentSnapshot.tDx, currentSnapshot.iY - 1)))
                                    {
                                        retVal= new GridPos(currentSnapshot.iX, currentSnapshot.iY);
                                        continue;
                                    }
                                }
                                else
                                {
                                    if ((iParam.SearchGrid.IsWalkableAt(currentSnapshot.iX + 1, currentSnapshot.iY) && !iParam.SearchGrid.IsWalkableAt(currentSnapshot.iX + 1, currentSnapshot.iY - currentSnapshot.tDy)) ||
                                        (iParam.SearchGrid.IsWalkableAt(currentSnapshot.iX - 1, currentSnapshot.iY) && !iParam.SearchGrid.IsWalkableAt(currentSnapshot.iX - 1, currentSnapshot.iY - currentSnapshot.tDy)))
                                    {
                                        retVal= new GridPos(currentSnapshot.iX, currentSnapshot.iY);
                                        continue;
                                    }
                                }
                            }
             

                            // when moving diagonally, must check for vertical/horizontal jump points
                            if (currentSnapshot.tDx != 0 && currentSnapshot.tDy != 0)
                            {
                                currentSnapshot.stage = 3;
                                stack.Push(currentSnapshot);

                                newSnapshot = new JumpSnapshot();
                                newSnapshot.iX = currentSnapshot.iX + currentSnapshot.tDx;
                                newSnapshot.iY = currentSnapshot.iY;
                                newSnapshot.iPx = currentSnapshot.iX;
                                newSnapshot.iPy = currentSnapshot.iY;
                                newSnapshot.stage = 0;
                                stack.Push(newSnapshot);
                                continue;
                            }

                            // moving diagonally, must make sure both of the vertical/horizontal
                            // neighbors is open to allow the path
                            if (iParam.SearchGrid.IsWalkableAt(currentSnapshot.iX + currentSnapshot.tDx, currentSnapshot.iY) && iParam.SearchGrid.IsWalkableAt(currentSnapshot.iX, currentSnapshot.iY + currentSnapshot.tDy))
                            {
                                newSnapshot = new JumpSnapshot();
                                newSnapshot.iX = currentSnapshot.iX + currentSnapshot.tDx;
                                newSnapshot.iY = currentSnapshot.iY + currentSnapshot.tDy;
                                newSnapshot.iPx = currentSnapshot.iX;
                                newSnapshot.iPy = currentSnapshot.iY;
                                newSnapshot.stage = 0;
                                stack.Push(newSnapshot);
                                continue;
                            }
                        }
                        retVal = null;
                        break;
                    case 1:
                        currentSnapshot.jx = retVal;

                        currentSnapshot.stage = 2;
                        stack.Push(currentSnapshot);

                        newSnapshot = new JumpSnapshot();
                        newSnapshot.iX = currentSnapshot.iX;
                        newSnapshot.iY = currentSnapshot.iY + currentSnapshot.tDy;
                        newSnapshot.iPx = currentSnapshot.iX;
                        newSnapshot.iPy = currentSnapshot.iY;
                        newSnapshot.stage = 0;
                        stack.Push(newSnapshot);
                        break;
                    case 2:
                        currentSnapshot.jy = retVal;
                        if (currentSnapshot.jx != null || currentSnapshot.jy != null)
                        {
                            retVal = new GridPos(currentSnapshot.iX, currentSnapshot.iY);
                            continue;
                        }

                        // moving diagonally, must make sure one of the vertical/horizontal
                        // neighbors is open to allow the path
                        if (iParam.SearchGrid.IsWalkableAt(currentSnapshot.iX + currentSnapshot.tDx, currentSnapshot.iY) || iParam.SearchGrid.IsWalkableAt(currentSnapshot.iX, currentSnapshot.iY + currentSnapshot.tDy))
                        {
                            newSnapshot = new JumpSnapshot();
                            newSnapshot.iX = currentSnapshot.iX + currentSnapshot.tDx;
                            newSnapshot.iY = currentSnapshot.iY + currentSnapshot.tDy;
                            newSnapshot.iPx = currentSnapshot.iX;
                            newSnapshot.iPy = currentSnapshot.iY;
                            newSnapshot.stage = 0;
                            stack.Push(newSnapshot);
                            continue;
                        }
                        else if (iParam.CrossAdjacentPoint)
                        {
                            newSnapshot = new JumpSnapshot();
                            newSnapshot.iX = currentSnapshot.iX + currentSnapshot.tDx;
                            newSnapshot.iY = currentSnapshot.iY + currentSnapshot.tDy;
                            newSnapshot.iPx = currentSnapshot.iX;
                            newSnapshot.iPy = currentSnapshot.iY;
                            newSnapshot.stage = 0;
                            stack.Push(newSnapshot);
                            continue;
                        }
                        retVal = null;
                        break;
                    case 3:
                        currentSnapshot.jx = retVal;

                        currentSnapshot.stage = 4;
                        stack.Push(currentSnapshot);

                        newSnapshot = new JumpSnapshot();
                        newSnapshot.iX = currentSnapshot.iX;
                        newSnapshot.iY = currentSnapshot.iY + currentSnapshot.tDy;
                        newSnapshot.iPx = currentSnapshot.iX;
                        newSnapshot.iPy = currentSnapshot.iY;
                        newSnapshot.stage = 0;
                        stack.Push(newSnapshot);
                        break;
                    case 4:
                        currentSnapshot.jy = retVal;
                        if (currentSnapshot.jx != null || currentSnapshot.jy != null)
                        {
                            retVal = new GridPos(currentSnapshot.iX, currentSnapshot.iY);
                            continue;
                        }

                        // moving diagonally, must make sure both of the vertical/horizontal
                        // neighbors is open to allow the path
                        if (iParam.SearchGrid.IsWalkableAt(currentSnapshot.iX + currentSnapshot.tDx, currentSnapshot.iY) && iParam.SearchGrid.IsWalkableAt(currentSnapshot.iX, currentSnapshot.iY + currentSnapshot.tDy))
                        {
                            newSnapshot = new JumpSnapshot();
                            newSnapshot.iX = currentSnapshot.iX + currentSnapshot.tDx;
                            newSnapshot.iY = currentSnapshot.iY + currentSnapshot.tDy;
                            newSnapshot.iPx = currentSnapshot.iX;
                            newSnapshot.iPy = currentSnapshot.iY;
                            newSnapshot.stage = 0;
                            stack.Push(newSnapshot);
                            continue;
                        }
                        retVal = null;
                        break;
                }
            }

            return retVal;

        }
        private static GridPos? jump(JumpPointParam iParam, int iX, int iY, int iPx, int iPy)
        {
            if (!iParam.SearchGrid.IsWalkableAt(iX, iY))
            {
                return null;
            }
            else if (iParam.SearchGrid.GetNodeAt(iX, iY).Equals(iParam.EndNode))
            {
                return new GridPos(iX, iY);
            }
        
            int tDx = iX - iPx;
            int tDy = iY - iPy;
            GridPos? jx=null;
            GridPos? jy=null;
            if (iParam.CrossCorner)
            {
                // check for forced neighbors
                // along the diagonal
                if (tDx != 0 && tDy != 0)
                {
                    if ((iParam.SearchGrid.IsWalkableAt(iX - tDx, iY + tDy) && !iParam.SearchGrid.IsWalkableAt(iX - tDx, iY)) ||
                        (iParam.SearchGrid.IsWalkableAt(iX + tDx, iY - tDy) && !iParam.SearchGrid.IsWalkableAt(iX, iY - tDy)))
                    {
                        return new GridPos(iX, iY);
                    }
                }
                // horizontally/vertically
                else
                {
                    if (tDx != 0)
                    { 
                        // moving along x
                        if ((iParam.SearchGrid.IsWalkableAt(iX + tDx, iY + 1) && !iParam.SearchGrid.IsWalkableAt(iX, iY + 1)) ||
                            (iParam.SearchGrid.IsWalkableAt(iX + tDx, iY - 1) && !iParam.SearchGrid.IsWalkableAt(iX, iY - 1)))
                        {
                            return new GridPos(iX, iY);
                        }
                    }
                    else
                    {
                        if ((iParam.SearchGrid.IsWalkableAt(iX + 1, iY + tDy) && !iParam.SearchGrid.IsWalkableAt(iX + 1, iY)) ||
                            (iParam.SearchGrid.IsWalkableAt(iX - 1, iY + tDy) && !iParam.SearchGrid.IsWalkableAt(iX - 1, iY)))
                        {
                            return new GridPos(iX, iY);
                        }
                    }
                }
                // when moving diagonally, must check for vertical/horizontal jump points
                if (tDx != 0 && tDy != 0)
                {
                    jx = jump(iParam, iX + tDx, iY, iX, iY);
                    jy = jump(iParam, iX, iY + tDy, iX, iY);
                    if (jx != null || jy != null)
                    {
                        return new GridPos(iX, iY);
                    }
                }

                // moving diagonally, must make sure one of the vertical/horizontal
                // neighbors is open to allow the path
                if (iParam.SearchGrid.IsWalkableAt(iX + tDx, iY) || iParam.SearchGrid.IsWalkableAt(iX, iY + tDy))
                {
                    return jump(iParam, iX + tDx, iY + tDy, iX, iY);
                }
                else if (iParam.CrossAdjacentPoint)
                {
                    return jump(iParam, iX + tDx, iY + tDy, iX, iY);
                }
                else
                {
                    return null;
                }
            }
            else //if (!iParam.CrossCorner)
            {
                // check for forced neighbors
                // along the diagonal
                if (tDx != 0 && tDy != 0)
                {
                    if ((iParam.SearchGrid.IsWalkableAt(iX + tDx, iY + tDy) && iParam.SearchGrid.IsWalkableAt(iX, iY + tDy) && !iParam.SearchGrid.IsWalkableAt(iX + tDx, iY)) ||
                        (iParam.SearchGrid.IsWalkableAt(iX + tDx, iY + tDy) && iParam.SearchGrid.IsWalkableAt(iX + tDx, iY) && !iParam.SearchGrid.IsWalkableAt(iX, iY + tDy)))
                    {
                        return new GridPos(iX, iY);
                    }
                }
                // horizontally/vertically
                else
                {
                    if (tDx != 0)
                    { 
                        // moving along x
                        if ((iParam.SearchGrid.IsWalkableAt(iX, iY + 1) && !iParam.SearchGrid.IsWalkableAt(iX - tDx, iY + 1)) ||
                            (iParam.SearchGrid.IsWalkableAt(iX, iY - 1) && !iParam.SearchGrid.IsWalkableAt(iX - tDx, iY - 1)))
                        {
                            return new GridPos(iX, iY);
                        }
                    }
                    else
                    {
                        if ((iParam.SearchGrid.IsWalkableAt(iX + 1, iY) && !iParam.SearchGrid.IsWalkableAt(iX + 1, iY - tDy)) ||
                            (iParam.SearchGrid.IsWalkableAt(iX - 1, iY) && !iParam.SearchGrid.IsWalkableAt(iX - 1, iY - tDy)))
                        {
                            return new GridPos(iX, iY);
                        }
                    }
                }
             

                // when moving diagonally, must check for vertical/horizontal jump points
                if (tDx != 0 && tDy != 0)
                {
                    jx = jump(iParam, iX + tDx, iY, iX, iY);
                    jy = jump(iParam, iX, iY + tDy, iX, iY);
                    if (jx != null || jy != null)
                    {
                        return new GridPos(iX, iY);
                    }
                }

                // moving diagonally, must make sure both of the vertical/horizontal
                // neighbors is open to allow the path
                if (iParam.SearchGrid.IsWalkableAt(iX + tDx, iY) && iParam.SearchGrid.IsWalkableAt(iX, iY + tDy))
                {
                    return jump(iParam, iX + tDx, iY + tDy, iX, iY);
                }
                else
                {
                    return null;
                }
            }
           
        }

        private static List<GridPos> findNeighbors(JumpPointParam iParam, Node iNode)
        {
            Node tParent = (Node)iNode.parent;
            int tX = iNode.x;
            int tY = iNode.y;
            int tPx, tPy, tDx, tDy;
            List<GridPos> tNeighbors = new List<GridPos>();
            List<Node> tNeighborNodes;
            Node tNeighborNode;

            // directed pruning: can ignore most neighbors, unless forced.
            if (tParent != null)
            {
                tPx = tParent.x;
                tPy = tParent.y;
                // get the normalized direction of travel
                tDx = (tX - tPx) / Math.Max(Math.Abs(tX - tPx), 1);
                tDy = (tY - tPy) / Math.Max(Math.Abs(tY - tPy), 1);

                if (iParam.CrossCorner)
                {
                    // search diagonally
                    if (tDx != 0 && tDy != 0)
                    {
                        if (iParam.SearchGrid.IsWalkableAt(tX, tY + tDy))
                        {
                            tNeighbors.Add(new GridPos(tX, tY + tDy));
                        }
                        if (iParam.SearchGrid.IsWalkableAt(tX + tDx, tY))
                        {
                            tNeighbors.Add(new GridPos(tX + tDx, tY));
                        }

                        if (iParam.SearchGrid.IsWalkableAt(tX + tDx, tY + tDy))
                        {
                            if (iParam.SearchGrid.IsWalkableAt(tX, tY + tDy) || iParam.SearchGrid.IsWalkableAt(tX + tDx, tY))
                            {
                                tNeighbors.Add(new GridPos(tX + tDx, tY + tDy));
                            }
                            else if (iParam.CrossAdjacentPoint)
                            {
                                tNeighbors.Add(new GridPos(tX + tDx, tY + tDy));
                            }
                        }

                        if (iParam.SearchGrid.IsWalkableAt(tX - tDx, tY + tDy))
                        {
                            if (iParam.SearchGrid.IsWalkableAt(tX, tY + tDy) && !iParam.SearchGrid.IsWalkableAt(tX - tDx, tY))
                            {
                                tNeighbors.Add(new GridPos(tX - tDx, tY + tDy));
                            }
                        }

                        if (iParam.SearchGrid.IsWalkableAt(tX + tDx, tY - tDy))
                        {
                            if (iParam.SearchGrid.IsWalkableAt(tX + tDx, tY) && !iParam.SearchGrid.IsWalkableAt(tX, tY - tDy))
                            {
                                tNeighbors.Add(new GridPos(tX + tDx, tY - tDy));
                            }
                        }


                    }
                    // search horizontally/vertically
                    else
                    {
                        if (tDx == 0)
                        {
                            if (iParam.SearchGrid.IsWalkableAt(tX, tY + tDy))
                            {
                                tNeighbors.Add(new GridPos(tX, tY + tDy));

                                if (iParam.SearchGrid.IsWalkableAt(tX + 1, tY + tDy) && !iParam.SearchGrid.IsWalkableAt(tX + 1, tY))
                                {
                                    tNeighbors.Add(new GridPos(tX + 1, tY + tDy));
                                }
                                if (iParam.SearchGrid.IsWalkableAt(tX - 1, tY + tDy) && !iParam.SearchGrid.IsWalkableAt(tX - 1, tY))
                                {
                                    tNeighbors.Add(new GridPos(tX - 1, tY + tDy));
                                }
                            }
                            else if (iParam.CrossAdjacentPoint)
                            {
                                if (iParam.SearchGrid.IsWalkableAt(tX + 1, tY + tDy) && !iParam.SearchGrid.IsWalkableAt(tX + 1, tY))
                                {
                                    tNeighbors.Add(new GridPos(tX + 1, tY + tDy));
                                }
                                if (iParam.SearchGrid.IsWalkableAt(tX - 1, tY + tDy) && !iParam.SearchGrid.IsWalkableAt(tX - 1, tY))
                                {
                                    tNeighbors.Add(new GridPos(tX - 1, tY + tDy));
                                }
                            }
                        }
                        else
                        {
                            if (iParam.SearchGrid.IsWalkableAt(tX + tDx, tY))
                            {

                                tNeighbors.Add(new GridPos(tX + tDx, tY));

                                if (iParam.SearchGrid.IsWalkableAt(tX + tDx, tY + 1) && !iParam.SearchGrid.IsWalkableAt(tX, tY + 1))
                                {
                                    tNeighbors.Add(new GridPos(tX + tDx, tY + 1));
                                }
                                if (iParam.SearchGrid.IsWalkableAt(tX + tDx, tY - 1) && !iParam.SearchGrid.IsWalkableAt(tX, tY - 1))
                                {
                                    tNeighbors.Add(new GridPos(tX + tDx, tY - 1));
                                }
                            }
                            else if (iParam.CrossAdjacentPoint)
                            {
                                if (iParam.SearchGrid.IsWalkableAt(tX + tDx, tY + 1) && !iParam.SearchGrid.IsWalkableAt(tX, tY + 1))
                                {
                                    tNeighbors.Add(new GridPos(tX + tDx, tY + 1));
                                }
                                if (iParam.SearchGrid.IsWalkableAt(tX + tDx, tY - 1) && !iParam.SearchGrid.IsWalkableAt(tX, tY - 1))
                                {
                                    tNeighbors.Add(new GridPos(tX + tDx, tY - 1));
                                }
                            }
                        }
                    }
                }
                else // if(!iParam.CrossCorner)
                {
                    // search diagonally
                    if (tDx != 0 && tDy != 0)
                    {
                        if (iParam.SearchGrid.IsWalkableAt(tX, tY + tDy))
                        {
                            tNeighbors.Add(new GridPos(tX, tY + tDy));
                        }
                        if (iParam.SearchGrid.IsWalkableAt(tX + tDx, tY))
                        {
                            tNeighbors.Add(new GridPos(tX + tDx, tY));
                        }

                        if (iParam.SearchGrid.IsWalkableAt(tX + tDx, tY + tDy))
                        {
                            if (iParam.SearchGrid.IsWalkableAt(tX, tY + tDy) && iParam.SearchGrid.IsWalkableAt(tX + tDx, tY))
                                tNeighbors.Add(new GridPos(tX + tDx, tY + tDy));
                        }

                        if (iParam.SearchGrid.IsWalkableAt(tX - tDx, tY + tDy))
                        {
                            if (iParam.SearchGrid.IsWalkableAt(tX, tY + tDy) && iParam.SearchGrid.IsWalkableAt(tX - tDx, tY))
                                tNeighbors.Add(new GridPos(tX - tDx, tY + tDy));
                        }

                        if (iParam.SearchGrid.IsWalkableAt(tX + tDx, tY - tDy))
                        {
                            if (iParam.SearchGrid.IsWalkableAt(tX, tY - tDy) && iParam.SearchGrid.IsWalkableAt(tX + tDx, tY))
                                tNeighbors.Add(new GridPos(tX + tDx, tY - tDy));
                        }


                    }
                    // search horizontally/vertically
                    else
                    {
                        if (tDx == 0)
                        {
                            if (iParam.SearchGrid.IsWalkableAt(tX, tY + tDy))
                            {
                                tNeighbors.Add(new GridPos(tX, tY + tDy));

                                if (iParam.SearchGrid.IsWalkableAt(tX + 1, tY + tDy) && iParam.SearchGrid.IsWalkableAt(tX + 1, tY))
                                {
                                    tNeighbors.Add(new GridPos(tX + 1, tY + tDy));
                                }
                                if (iParam.SearchGrid.IsWalkableAt(tX - 1, tY + tDy) && iParam.SearchGrid.IsWalkableAt(tX - 1, tY))
                                {
                                    tNeighbors.Add(new GridPos(tX - 1, tY + tDy));
                                }
                            }
                            if (iParam.SearchGrid.IsWalkableAt(tX + 1, tY))
                                tNeighbors.Add(new GridPos(tX + 1, tY));
                            if (iParam.SearchGrid.IsWalkableAt(tX - 1, tY))
                                tNeighbors.Add(new GridPos(tX - 1, tY));
                        }
                        else
                        {
                            if (iParam.SearchGrid.IsWalkableAt(tX + tDx, tY))
                            {

                                tNeighbors.Add(new GridPos(tX + tDx, tY));

                                if (iParam.SearchGrid.IsWalkableAt(tX + tDx, tY + 1) && iParam.SearchGrid.IsWalkableAt(tX, tY + 1))
                                {
                                    tNeighbors.Add(new GridPos(tX + tDx, tY + 1));
                                }
                                if (iParam.SearchGrid.IsWalkableAt(tX + tDx, tY - 1) && iParam.SearchGrid.IsWalkableAt(tX, tY - 1))
                                {
                                    tNeighbors.Add(new GridPos(tX + tDx, tY - 1));
                                }
                            }
                            if (iParam.SearchGrid.IsWalkableAt(tX, tY + 1))
                                tNeighbors.Add(new GridPos(tX, tY + 1));
                            if (iParam.SearchGrid.IsWalkableAt(tX, tY - 1))
                                tNeighbors.Add(new GridPos(tX, tY - 1));
                        }
                    }
                }
             
            }
            // return all neighbors
            else
            {
                tNeighborNodes = iParam.SearchGrid.GetNeighbors(iNode, iParam.CrossCorner, iParam.CrossAdjacentPoint);
                for (int i = 0; i < tNeighborNodes.Count; i++)
                {
                    tNeighborNode = tNeighborNodes[i];
                    tNeighbors.Add(new GridPos(tNeighborNode.x, tNeighborNode.y));
                }
            }

            return tNeighbors;
        }
    }
}
public class THData
{
    /// 

    /// Function for doing a deep clone of an object.
    /// 

    /// Returns a deep-copy clone of the object.
    public virtual THData DeepClone()
    {
        //First do a shallow copy.
        THData returnData = (THData)this.MemberwiseClone();

        //Get the type.
        Type type = returnData.GetType();

        //Now get all the member variables.
        FieldInfo[] fieldInfoArray = type.GetFields();

        //Deepclone members that extend THData.
        //This will ensure we get everything we need.
        foreach (FieldInfo fieldInfo in fieldInfoArray)
        {
            //This gets the actual object in that field.
            object sourceFieldValue = fieldInfo.GetValue(this);

            //See if this member is THData
            if (sourceFieldValue is THData)
            {
                //If so, cast as a THData.
                THData sourceTHData = (THData)sourceFieldValue;

                //Create a clone of it.
                THData clonedTHData = sourceTHData.DeepClone();

                //Within the cloned containig class.
                fieldInfo.SetValue(returnData, clonedTHData);
            }
        }
        return returnData;
    }
}
