using System;
using System.Security.Cryptography;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Chess.Scripts.Core
{
    public class ChessPlayerPlacementHandler : MonoBehaviour
    {
        [SerializeField] public int row, column;
        public GameObject controller;
        private int px;
        private int py;
        Vector3[] peicepositions = new Vector3[16];

        private void Start()
        {
            transform.position = ChessBoardPlacementHandler.Instance.GetTile(row, column).transform.position;
            //Get the game controller
            controller = GameObject.FindGameObjectWithTag("GameController");
        }
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    px = (int)math.round(hit.transform.position.y + 3.5);
                    py = (int)math.round(hit.transform.position.x + 3.5) ;
                    
                    //Select ChessPeice    
                    if (hit.transform.name == "King")
                    {
                        Clearhighlight(); // clear previous highlights
                        InitiateHighlight("King"); // highlight the tiles
                    }
                    if (hit.transform.name == "Pawn1" || hit.transform.name == "Pawn2" || hit.transform.name == "Pawn3" || hit.transform.name == "Pawn4"
                        || hit.transform.name == "Pawn5" || hit.transform.name == "Pawn6" || hit.transform.name == "Pawn7" || hit.transform.name == "Pawn8")
                    {
                        Clearhighlight();
                        InitiateHighlight("Pawn1");
                    }
                    if (hit.transform.name == "Bishop" || hit.transform.name == "Bishop1")
                    {
                        Clearhighlight();
                        InitiateHighlight("Bishop");
                    }
                    if (hit.transform.name == "Queen")
                    {
                        Clearhighlight();
                        InitiateHighlight("Queen");
                    }
                    if (hit.transform.name == "Knight" || hit.transform.name == "Knight1")
                    {
                        Clearhighlight();
                        InitiateHighlight("Knight");
                    }
                    if (hit.transform.name == "Rook" || hit.transform.name == "Rook1")
                    {
                        Clearhighlight();
                        InitiateHighlight("Rook");  
                    }
                }
            }
        }
        public void Clearhighlight()
        {
            ChessBoardPlacementHandler.Instance.ClearHighlights();
        }
        public void InitiateHighlight(string x)
        {
            switch (x)
            {
                case "Queen":
                  
                    Highlight_block(1, 0);
                    Highlight_block(0, 1);
                    Highlight_block(1, 1);
                    Highlight_block(-1, 0);
                    Highlight_block(0, -1);
                    Highlight_block(-1, -1);
                    Highlight_block(-1, 1);
                    Highlight_block(1, -1);
               
                    break;

                case "King":

                    surround(1,0);
                    surround(0, 1);
                    surround(1, 1);
                    surround(-1, 0);
                    surround(0, -1);
                    surround(1, -1);
                    surround(-1, 1);
                    surround(-1, -1);
                    break;

                case "Knight":
                case "knight1":
                    LHighlight_block(2,1);
                    LHighlight_block(2,-1);
                    LHighlight_block(-2, 1);
                    LHighlight_block(-2, -1);
                    LHighlight_block(1, 2);
                    LHighlight_block(-1, 2);
                    LHighlight_block(1,-2);
                    LHighlight_block(-1,-2);
                    break;

                case "Bishop":
                case "Bishop1":


                    Highlight_block(1, 1);
                    Highlight_block(1, -1);
                    Highlight_block(-1, 1);
                    Highlight_block(-1, -1);
                    break;

                case "Rook":
                case "Rook1":

                    Highlight_block(1, 0);
                    Highlight_block(0, 1);
                    Highlight_block(-1, 0);
                    Highlight_block(0, -1);
                    break;

                case "Pawn1":
                case "Pawn2":
                case "Pawn3":
                case "Pawn4":
                case "Pawn5":
                case "Pawn6":
                case "Pawn7":
                case "Pawn8":

                    singlemove(1,0);
                    break;
            }
        }
        public bool nopiecesOverlap = true;
        public void isoccupied(int x,int y)
        {
            GameObject[] chesspeices = GameObject.FindGameObjectsWithTag("Black");

            for (int i = 0; i < 15; i++)
            {
                peicepositions[i] = chesspeices[i].transform.position;
            }
            ChessBoardPlacementHandler handler = controller.GetComponent<ChessBoardPlacementHandler>();
            GameObject tile = handler.GetTile(x, y);
            for (int i = 0; i < 15; i++)
            {
                nopiecesOverlap = true;
                if ((int)math.round(tile.transform.position.x+3.5) == (int)math.round(peicepositions[i].x+3.5) && (int)math.round(tile.transform.position.y+3.5) == (int)math.round(peicepositions[i].y + 3.5))
                {
                    nopiecesOverlap = false;
                        break;
                    }
            }
        }
        public void Highlight_block(int incr_x, int incr_y)
        {
            ChessBoardPlacementHandler handler = controller.GetComponent<ChessBoardPlacementHandler>();

            int x =  px+incr_x;
            int y =  py+incr_y;
            
            while (x >= 0 && x < 8 && y >= 0 && y < 8)
            {
                isoccupied(x,y);

                if (nopiecesOverlap)
                {
                    handler.Highlight(x, y);
                }
                else
                {
                    break;
                }
                x += incr_x;
                y += incr_y;
            }
        }

        public void LHighlight_block(int incr_x, int incr_y)
        {
            ChessBoardPlacementHandler handler = controller.GetComponent<ChessBoardPlacementHandler>();

            int x = px + incr_x;
            int y = py + incr_y;

            while (x >= px - 3 && x < px + 3 && y >= py - 3 && y < py + 3 && y >= 0 && x >= 0 && x < 8 && y < 8)
            {
                isoccupied(x,y);

                if (nopiecesOverlap) // if tile has no pieces on it
                {
                    handler.Highlight(x, y); // highlight the tile
                }
                else // if tile has a piece on it
                {
                    // check if the piece on the tile is of the opposite color

                    break; // break out of the loop, as pieces cannot move beyond an occupied tile
                }
                x += incr_x;
                y += incr_y;
            } 
        }

        public void singlemove(int x, int y)
        {
            isoccupied(px+x, py+y);
         
            ChessBoardPlacementHandler handler = controller.GetComponent<ChessBoardPlacementHandler>();
            if (nopiecesOverlap)
            {
                if (px == 1)
                {
                    handler.Highlight(px + x, py + y);
                    isoccupied(px + x + 1, py + y);
                    if (nopiecesOverlap)
                    {
                        handler.Highlight(px + x + 1, py + y);
                    }
                }
                else
                {
                    handler.Highlight(px + x, py + y);
                }
            }
        }
        public void surround(int x, int y)
        {
            ChessBoardPlacementHandler handler = controller.GetComponent<ChessBoardPlacementHandler>();
            int x1 = px+x, y1 = py+y;
            isoccupied(x1, y1);
            if (nopiecesOverlap)
            {
                if (x1 >= 0 && x1 < 8 && y1 >= 0 && y1 < 8)
                {
                    handler.Highlight(px + x, py + y);
                }
            }
        }       
    }
}
   

