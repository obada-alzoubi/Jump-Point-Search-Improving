/*! 
@file SearchGridForm.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eppathfinding>
@date July 16, 2013
@brief SearchGridForm Interface
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

An Interface for the SearchGridForm Class.

*/
using System;
using System.Collections.Generic;
using General;
using EpPathFinding;
using PathFinder;

namespace PathFind
{
    public  class SearchGridForm 
    {
         int width = 64;
         int height = 32;
        State[,] map;
        GridBox[][] m_rectangles,coloneSearchGrid;
        List<ResultBox> m_resultBox;
        List<GridLine> m_resultLine;

        //GridBox m_lastBoxSelect;
        //BoxType m_lastBoxType;
        public  long countSearch = 0;
        BaseGrid searchGrid;
        JumpPointParam jumpParam,CloneParam;
        ///
        public double cost = 0;
        public int nExp = 0;
        public List<GridPos> Path;
        public bool HitBorder = false;
        public  List<Node> CopiedOpenList;
        public List<Node> CopiedClosedList;

        ///
        public List<Node> CopiedOpenListT;
        public System.Diagnostics.Stopwatch sw=new System.Diagnostics.Stopwatch();
        int x_start;
        int y_start;
        int x_end;
        int y_end;



        public SearchGridForm( int width, int height, int x_start,int y_start, int x_end, int y_end)
        {

            this.map = map;
            this.width = width;
            this.height = height;
            this.x_start = x_start;
            this.y_start = y_start;
            this.x_end = x_end;
            this.y_end = y_end;
            //InitializeComponent();
            //this.DoubleBuffered = true;

            m_resultBox = new List<ResultBox>();
            //this.Width = (width+1) * 20;
            //this.Height = (height+1) * 20 +100;
            //this.MaximumSize = new Size(this.Width, this.Height);
            //this.MaximizeBox = false;


            //m_rectangles = new GridBox[width][];
            

            //m_resultLine = new List<GridLine>();

            //Grid searchGrid=new Grid(width,height,movableMatrix);
            //BaseGrid searchGrid = new StaticGrid(width, height, movableMatrix);
            //searchGrid = new DynamicGrid();
            searchGrid = new DynamicGridWPool(SingletonHolder<NodePool>.Instance);
            jumpParam = new JumpPointParam(searchGrid, true, false, false, HeuristicMode.EUCLIDEAN);//new JumpPointParam(searchGrid, startPos, endPos, cbCrossCorners.Checked, HeuristicMode.EUCLIDEANSQR);
            jumpParam.UseRecursive = false;
            
        }

        // private void btnSearch_Click(object sender, EventArgs e)
        public  void btn(int numberCuts, List<Node> inlist, int[,] borders,State[,] map)
        {



            GridPos startPos = new GridPos();
            GridPos endPos = new GridPos();
            //int[,] borders = new int[width, height];
            int counter = 0;
            for (int widthTrav = 0; widthTrav < width; widthTrav++)
                {
                    for (int heightTrav = 0; heightTrav < height; heightTrav++)
                    {
                        if (map[widthTrav, heightTrav].value >=1 )
                        {
                            searchGrid.SetWalkableAt(new GridPos(widthTrav, heightTrav), true);
                        counter = counter + 1;

                        /// fILL INDEXES ABOUT BORDERS STATE
                        if (map[widthTrav, heightTrav].value >= 2)
                            {
                                //borders[widthTrav, heightTrav] = 1;
                                //counter = counter + 1;
                            }
                        ///
                        }
                        else
                        {
                            searchGrid.SetWalkableAt(new GridPos(widthTrav, heightTrav), false);
                        counter = counter + 1;

                    }

                    if (widthTrav== x_start & heightTrav==y_start )
                        {
                            startPos.x=widthTrav;
                            startPos.y=heightTrav;
                        }
                        if(widthTrav == x_end & heightTrav == y_end)
                        {
                            endPos.x=widthTrav;
                            endPos.y=heightTrav;
                        }

                    }
                }
            //Console.WriteLine("No. of all states {0}", counter);
            //
            //Console.WriteLine("map dimension {0} * {1}", jumpParam.SearchGrid.width ,jumpParam.SearchGrid.height);


            //Obada
            sw.Reset();
            sw.Start();
            //numberCuts = 1;
            if (numberCuts == 1)
            {
                jumpParam.CrossCorner = false;
                jumpParam.CrossAdjacentPoint = false;
                jumpParam.UseRecursive = false;
                jumpParam.Reset(startPos, endPos);
            }
            //else
            //{
            //    jumpParam.CrossCorner = false;
            //    jumpParam.CrossAdjacentPoint = false;
            //    jumpParam.UseRecursive = false;
            //    jumpParam.Reset(startPos, endPos);

            //    foreach (Node node in CopiedOpenListT)
            //    {
            //        Node JNode;
            //        GridPos Pnode = new GridPos(node.x, node.y);
            //        try
            //        {
            //            JNode = jumpParam.SearchGrid.GetNodeAt(Pnode);
            //            JNode.isClosed = node.isClosed;
            //            JNode.isOpened = node.isOpened;
            //            JNode.parent = node.parent;
            //            JNode.heuristicCurNodeToEndLen = node.heuristicCurNodeToEndLen;
            //            JNode.heuristicStartToEndLen = node.heuristicStartToEndLen;
            //            //if (JNode.startToCurNodeLen > 0)
            //              //  Console.WriteLine("Finally there is sth here");
            //            JNode.startToCurNodeLen = node.startToCurNodeLen;
            //        }
            //        catch
            //        {
            //            //Console.WriteLine("At open List: problem with node ({0},{1})",node.x,node.y);
            //        }
            //    }
            //    foreach (Node node in CopiedClosedList)
            //    {
            //        Node JNode;
            //        GridPos Pnode = new GridPos(node.x, node.y);
            //        try
            //        {
            //            JNode = jumpParam.SearchGrid.GetNodeAt(Pnode);
            //            JNode.isClosed = node.isClosed;
            //            JNode.isOpened = node.isOpened;
            //            JNode.parent = node.parent;
            //            JNode.heuristicCurNodeToEndLen = node.heuristicCurNodeToEndLen;
            //            JNode.heuristicStartToEndLen = node.heuristicStartToEndLen;
            //            //if (JNode.startToCurNodeLen > 0)
            //              //  Console.WriteLine("Finally there is sth here");
            //            JNode.startToCurNodeLen = node.startToCurNodeLen;
            //        }
            //        catch
            //        {
            //            //Console.WriteLine("At Closed list: problem with node ({0},{1})", node.x, node.y);

            //        }
            //    }

            //}

            //Console.WriteLine("start Node ({0},{1})", startPos.x, startPos.y);
            //Console.WriteLine("Goal Node ({0},{1})", endPos.x, endPos.y);



            numberCuts = 1;
            JumpPointFinder J = new JumpPointFinder();
            List<GridPos> resultList = J.FindPath(jumpParam,borders,numberCuts, inlist);
            CopiedOpenListT = jumpParam.copyList;
            CopiedClosedList = jumpParam.cCList;
            //CloneParam = J.CloneParam;
            //CloneParam = J.CloneParam;


            sw.Stop();
            countSearch = sw.ElapsedMilliseconds;
            Path = resultList;
            double totalCost = 0;
            bool pathHitBorder = false;
            for (int resultTrav = 0; resultTrav < resultList.Count-1; resultTrav++)
            {
                double tempX, tempY;
                tempX = (double)(resultList[resultTrav + 1].x - resultList[resultTrav].x) * (resultList[resultTrav + 1].x - resultList[resultTrav].x);
                tempY = (double)(resultList[resultTrav + 1].y - resultList[resultTrav].y) * (resultList[resultTrav + 1].y - resultList[resultTrav].y);
                totalCost += Math.Sqrt(tempX + tempY);
                if (borders[resultList[resultTrav ].x , resultList[resultTrav ].y ]>= 1)
                    pathHitBorder = true;

            }
            
            //System.Console.WriteLine("Total cost: {0}", totalCost);
            //System.Console.WriteLine("Expanded states: {0}", J.numExp);
            nExp = J.numExp;
            cost = totalCost ;
            HitBorder = pathHitBorder;
            //return J.CopiedOpenListOut ;


        }



    }
}
