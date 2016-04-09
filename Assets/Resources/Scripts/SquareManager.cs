using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SquareManager : MonoBehaviour
{

    GameObject squareFolder;
    List<Square> squares;

    Queue<Square> queue;            // Add is enqueue, RemoveAt(0) is dequeue
    static int BOARDSIZEX = 24;
    static int BOARDSIZEY = 16;
    static int queueY = -2;
    Vector2 qpos1 = new Vector2(-5f, (float)queueY);
    Vector2 qpos2 = new Vector2(-4f, (float)queueY);
    Vector2 qpos3 = new Vector2(-3f, (float)queueY);
    Square[,] board;

    float counter = 0f;

    float push = 250f;


    public void init()
    {
        squareFolder = new GameObject();
        squares = new List<Square>();
        initQueue();        //initialize queue w/ 3 initial blocks
        initBoard();
    }


    public int getColor()
    {
        return Random.Range(0, 3);
    }


    public void initQueue()
    {
        queue = new Queue<Square>();
        //initialize first 3 blocks
        Square s1 = addSquare(qpos1, false);
        Square s2 = addSquare(qpos2, false);
        Square s3 = addSquare(qpos3, false);

        queue.Enqueue(s1);
        queue.Enqueue(s2);
        queue.Enqueue(s3);

    }

    public void initBoard()
    {
        board = new Square[BOARDSIZEX, BOARDSIZEY];
        //initialize level w/ ground squares (read from text file?)
        for (int i = 0; i < BOARDSIZEX; i++)
        {
            Square s = addSquare(new Vector2(i, 0), true);
            //			s.init (new Vector2 (i, 0), -1, true);
            board[i, 0] = s;

        }
    }

    //dequeues square and places it at pos, then updates queue
    public void placeSquare(Vector2 pos)
    {
        if (checkBounds(pos) && board[(int)pos.x, (int)pos.y] == null)
        {
            Square square = queue.Dequeue();
            square.setPosition(pos);
            board[(int)pos.x, (int)pos.y] = square;
            updateQueue();
            settleSquare(square);
            checkConflicts(square);
        }
        else {
            print("nO");
        }

    }

    public void settleSquare(Square s)
    {

        Vector2 pos = s.getPosition();
        Square below = board[(int)pos.x, (int)(pos.y - 1)];
        if (below == null)
        {
            counter = 0f;
            //("Square is at: " + s.getPosition ());

            board[(int)pos.x, (int)pos.y] = null;
            board[(int)pos.x, (int)pos.y - 1] = s;
            s.setPosition(new Vector2(pos.x, pos.y - 1));
            //("Square is at: " + s.getPosition ());

            settleSquare(s);
        }
    }

    public void checkConflicts(Square s)
    {
        Vector2 pos = s.getPosition();
        Square below = board[(int)pos.x, (int)(pos.y - 1)];
        Debug.Log("pos below is: "+ board[(int)pos.x, (int)(pos.y - 1)]);
        Square left = board[(int)(pos.x - 1), (int)pos.y];
        Debug.Log("pos left is: " + board[(int)(pos.x - 1), (int)pos.y]);
        Square right = board[(int)(pos.x + 1), (int)pos.y];
        Debug.Log("pos right is: " + board[(int)(pos.x + 1), (int)pos.y]);
        if (below.getColor() == s.getColor())
        {
            resolveConflict(s, below);
        }
        if (left != null && left.getColor() == s.getColor())
        {
            resolveConflict(s, left);
        }
        if (right != null && right.getColor() == s.getColor())
        {
            resolveConflict(s, right);
        }
    }

    public void resolveConflict(Square s, Square c)
    {
        Vector3 dir = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dir = dir.normalized;
        Debug.Log("dir is: " + dir);

        s.model.gameObject.GetComponent<Rigidbody2D>();

        s.model.rb.velocity = dir * push;
        s.model.rb.AddForce(dir * push);

        //Vector3 ScreenNormal = new Vector3(0, 0, 1);
        //Vector3 ForceDirection = Vector3.Cross(s.transform.position.normalized, ScreenNormal);
        //s.model.rb.AddForce(ForceDirection * push);



        //rb.AddForce(dir * pushPower, ForceMode2D.Force);

        /*Vector2 sPos = s.getPosition();
        Vector2 cPos = c.getPosition();
        board[(int)sPos.x, (int)sPos.y] = null;
        board[(int)cPos.x, (int)cPos.y] = null;
        Destroy(s.gameObject);
        Destroy(c.gameObject);

        s = board[(int)sPos.x, (int)(sPos.y + 1)];
        c = board[(int)cPos.x, (int)(cPos.y + 1)];
        resolveConflictHelper(s);
        resolveConflictHelper(c);*/
        /*while (s != null) {
			sPos = s.getPosition ();
			Square sAbove = board [(int)sPos.x, (int)(sPos.y + 1)];
			settleSquare (s);
			checkConflicts (s);
			s = sAbove;
		}
		while (c != null) {
			cPos = c.getPosition ();
			Square cAbove = board [(int)cPos.x, (int)(cPos.y + 1)];
			settleSquare (c);
			checkConflicts (c);
			c = cAbove;
		}*/
    }

    public void resolveConflictHelper(Square s)
    {
        while (s != null)
        {
            Vector2 sPos = s.getPosition();
            Square sAbove = board[(int)sPos.x, (int)(sPos.y + 1)];
            settleSquare(s);
            checkConflicts(s);
            s = sAbove;
        }
    }


    public bool checkBounds(Vector2 pos)
    {
        if ((pos.x >= 0 && pos.x < BOARDSIZEX) && (pos.y >= 0 && pos.y < BOARDSIZEY))
        {
            return true;
        }
        return false;

    }

    //change positions of the blocks in the queue, create a new block at position 3
    public void updateQueue()
    {
        Queue<Square> newq = new Queue<Square>();
        while (queue.Count > 0)
        {
            Square s = queue.Dequeue();
            s.setPosition(new Vector2(s.getPosition().x - 1f, queueY));
            newq.Enqueue(s);
        }
        queue = newq;
        Square end = addSquare(qpos3, false);
        queue.Enqueue(end);

    }


    public Square addSquare(Vector2 pos, bool isGround)
    {
        GameObject squareObject = new GameObject();
        Square square = squareObject.AddComponent<Square>();

        square.transform.parent = squareFolder.transform;
        square.transform.position = new Vector3(pos.x, pos.y, 0);
        if (isGround)
        {
            square.init(pos, -1, true);
        }
        else {
            square.init(pos, getColor(), false);
        }

        squares.Add(square);
        square.name = "Square " + squares.Count;

        return square;

    }
}