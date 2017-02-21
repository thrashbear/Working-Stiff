using UnityEngine;

public class GameController:MonoBehaviour {

    //Board
    public int[,] board;

    //Board blocks
    public Transform[] blocks;

    private Color blockColor = Color.white;

    void Start() {
        board = new int[10, 10];
        GenBoard();
    }

    void Update() {
        //Select block effect
        if(Block.select) {
            if(blockColor == Color.white) {
                blockColor = Block.select.gameObject.GetComponent<Renderer>().material.color;
            }
            //Change block color over time
            Block.select.gameObject.GetComponent<Renderer>().material.color = Color32.Lerp(blockColor, Color.black, Mathf.PingPong(Time.time, 1));
        }

        if(Block.select && Block.moveTo) { //If 2 blocks are selected, what we need more?
                                           //Check if they are near each other
            if(CheckIfNear() == true) {
                //Near
                Swap();//Swap their position and data

                //Check for match3
                if(CheckMatch() == true) {
                    //There is match
                    Block.select.gameObject.GetComponent<Renderer>().material.color = blockColor;
                    blockColor = Color.white;
                    Block.select = null;
                    Block.moveTo = null;
                } else {
                    //There is no match, return them in their default position
                    Swap();
                    Block.select.gameObject.GetComponent<Renderer>().material.color = blockColor;
                    blockColor = Color.white;
                    Block.select = null;
                    Block.moveTo = null;
                }
            } else {
                //Not near
                Block.select.gameObject.GetComponent<Renderer>().material.color = blockColor;
                blockColor = Color.white;
                Block.select = null;
                Block.moveTo = null;
                //We can again select new blocks
            }
        }
    }

    void GenBoard() {
        for(int x = 0; x < board.GetLength(0); x++) {
            for(int y = 0; y < board.GetLength(1); y++) {
                int randomNumber = Random.Range(0, blocks.Length); //ID
                Transform obj = (Transform)Instantiate(blocks[randomNumber].transform, new Vector3(x, y, 0), Quaternion.identity);
                obj.parent = transform;
                Block b = obj.gameObject.AddComponent<Block>();
                //Set values
                b.ID = randomNumber;
                b.x = x;
                b.y = y;
                //Set ID in board at this position
                board[x, y] = randomNumber;
            }
        }
    }

    bool CheckIfNear() {
        Block sel = Block.select.gameObject.GetComponent<Block>();
        Block mov = Block.moveTo.gameObject.GetComponent<Block>();

        //Check if near
        if(sel.x - 1 == mov.x && sel.y == mov.y) {
            //Left
            return true;
        }
        if(sel.x + 1 == mov.x && sel.y == mov.y) {
            //Right
            return true;
        }
        if(sel.x == mov.x && sel.y + 1 == mov.y) {
            //Up
            return true;
        }
        if(sel.x == mov.x && sel.y - 1 == mov.y) {
            //Down
            return true;
        }
        Debug.Log("What are you trying to select!");
        return false;
    }

    void Swap() {
        Block sel = Block.select.gameObject.GetComponent<Block>();
        Block mov = Block.moveTo.gameObject.GetComponent<Block>();

        Vector3 tempPos = sel.transform.position;
        int tempX = sel.x;
        int tempY = sel.y;

        //Swap their position
        sel.transform.position = mov.transform.position;
        mov.transform.position = tempPos;

        //Swap data
        sel.x = mov.x;
        sel.y = mov.y;

        mov.x = tempX;
        mov.y = tempY;

        //Change ID in board
        board[sel.x, sel.y] = sel.ID;
        board[mov.x, mov.y] = mov.ID;
    }

    bool CheckMatch() {
        //Get all blocks in scene
        Block[] allb = FindObjectsOfType(typeof(Block)) as Block[];
        Block sel = Block.select.gameObject.GetComponent<Block>();

        //Check how many blocks have same ID as our selected block(for each direction)
        int countU = 0; //Count Up
        int countD = 0; //Count Down
        int countL = 0; //Count Left
        int countR = 0; //Count RIght

        //Check how many same blocks have sam ID...
        //Left
        for(int l = sel.x - 1; l >= 0; l--) {
            if(board[l, sel.y] == sel.ID) {//If block have same ID
                countL++;
            }
            if(board[l, sel.y] != sel.ID) {//If block have same ID
                break;
            }
        }
        //Right
        for(int r = sel.x; r < board.GetLength(0); r++) {
            if(board[r, sel.y] == sel.ID) {//If block have same ID
                countR++;
            }
            if(board[r, sel.y] != sel.ID) {//If block have same ID
                break;
            }
        }
        //Down
        for(int d = sel.y - 1; d >= 0; d--) {
            if(board[sel.x, d] == sel.ID) {
                countD++;
            }
            if(board[sel.x, d] != sel.ID) {
                break;
            }
        }

        //Up
        for(int u = sel.y; u < board.GetLength(1); u++) {
            if(board[sel.x, u] == sel.ID) {
                countU++;
            }

            if(board[sel.x, u] != sel.ID) {
                break;
            }
        }

        //Check if there is 3+ match 
        if(countL + countR >= 3 || countD + countU >= 3) {
            if(countL + countR >= 3) {
                //Destroy and mark empty block
                for(int cl = 0; cl <= countL; cl++) {
                    foreach(Block b in allb) {
                        if(b.x == sel.x - cl && b.y == sel.y) {
                            Destroy(b.gameObject);
                            board[b.x, b.y] = 500; //To mark empty block
                        }
                    }
                }
                for(int cr = 0; cr < countR; cr++) {
                    foreach(Block b in allb) {
                        if(b.x == sel.x + cr && b.y == sel.y) {
                            Destroy(b.gameObject);
                            board[b.x, b.y] = 500; //To mark empty block
                        }
                    }
                }
            }
            if(countD + countU >= 3) {
                for(int cd = 0; cd <= countD; cd++) {
                    foreach(Block blc in allb) {
                        if(blc.x == sel.x && blc.y == sel.y - cd) {
                            board[blc.x, blc.y] = 500;
                            Destroy(blc.gameObject);
                        }
                    }
                }
                for(int cu = 0; cu < countU; cu++) {
                    foreach(Block blc in allb) {
                        if(blc.x == sel.x && blc.y == sel.y + cu) {
                            board[blc.x, blc.y] = 500;
                            Destroy(blc.gameObject);
                        }
                    }
                }
            }
            //Respawn blocks
            Respawn();
            return true;
        }
        return false;
    }

    void Respawn() {
        for(int x = 0; x < board.GetLength(0); x++) {
            for(int y = 0; y < board.GetLength(1); y++) {
                if(board[x, y] == 500) {
                    int randomNumber = Random.Range(0, blocks.Length); //ID
                    Transform obj = (Transform)Instantiate(blocks[randomNumber].transform, new Vector3(x, y, 0), Quaternion.identity);
                    obj.parent = transform;
                    Block b = obj.gameObject.AddComponent<Block>();
                    //Set values
                    b.ID = randomNumber;
                    b.x = x;
                    b.y = y;
                    //Set ID in board at this position
                    board[x, y] = randomNumber;
                }
            }
        }
    }
}