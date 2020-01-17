using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
/*
 *135行目ができていない 
 */

/*
 * クリックした場所から離した場所まで部屋を生成
 * 2階への切り替えも可能
 * 2階に切り替え時、地面の自動生成がされる
 * 部屋の削除も可能（tagを使用）
 */
public class tajigenn : MonoBehaviour
{
    public GameObject cube;
    public GameObject secondcube;
    public GameObject blackcube;
    public GameObject masu;

    public GameObject objfloor;
    public GameObject objsecond;

    private RaycastHit hit;

    string startObjectName = "";
    string endObjectName = "";


    //3次元の配列を置く
    int[,,] field = new int[10, 10, 10];
    int count = 0;

    //オブジェクト判定用の変数
    string Wall = "masu";

    //クリック用の変数
    Vector3 tmp = new Vector3(0, 0, 0);
    Vector3 tmp1 = new Vector3(0, 0, 0);
    Vector3 tmp2 = new Vector3(0, 0, 0);
    Vector3 tmp3 = new Vector3(0, 0, 0);


    int cubecount = 0;
    int blackcubecount = 0;
    int FirstfRoomCount = 1;
    int SecondRoomCount = 1;

    bool a = true;
    bool SecondStart = true;
    bool SecondGrandDel = false;

    private List<GameObject> WallTopList = new List<GameObject>();

    //オブジェクト削除用変数
    Vector3 TagPos = new Vector3(0, 0, 0);
    string gameObjName = "";

    GameObject[] SecondGrandDelTag;

    //Start is called before the first frame update
    void Start()
    {
        /*
         * 地面生成の処理
         * -----------------------------------------------------------------
         */
        // 1と0を代入
        // 初期化
        for (int i = 0; i < field.GetLength(0); i++)
        {
            for (int j = 0; j < field.GetLength(1); j++)
            {
                for (int k = 0; k < field.GetLength(2); k++)
                {
                    if (k == 0)
                    {
                        field[i, j, k] = 1;
                    }
                    else
                    {
                        field[i, j, k] = 0;
                    }
                }

            }
        }

        //各インデックスに代入された値をもとに生成するorしない

        for (int i = 0; i < field.GetLength(0); i++)
        {
            for (int j = 0; j < field.GetLength(1); j++)
            {
                for (int k = 0; k < field.GetLength(2); k++)
                {
                    //インデックスの値が1の時、cubeを生成
                    if (field[i, j, k] == 1)
                    {
                        GameObject wall = Instantiate(cube);
                        // wallの名前の変更 countによって一つ一つに番号をふる
                        wall.name = "cube" + count;
                        wall.transform.position = new Vector3(i, j, k);
                        count++;
                    }
                }
            }
        }
        /*
         * (地面生成処理)
         * -----------------------------------------------------------------
         */
    }


    //Update is called once per frame
    void Update()
    {
        // ボタンの読み込み
        objfloor = GameObject.Find("wall");
        objsecond = GameObject.Find("2F");

        /*
         * 部屋生成の処理
         * -----------------------------------------------------------------
         */
        // 部屋削除ボタンが表示されているとき
        if (objfloor != null)
        {
            // 2階ボタンが非表示の時（1階ボタンが表示されている = 1階モード）
            if (objsecond == null)
            {
                // 松瀬　01/17　追加
                if (SecondGrandDel == true)
                {
                    foreach(GameObject GrandDel in SecondGrandDelTag)
                    Destroy(SecondGrandDelTag[]);
                }
                //ボタンを押した処理
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    //スタートの座標を取得
                    if (Physics.Raycast(ray, out hit))
                    {
                        startObjectName = hit.collider.gameObject.name;

                        tmp = GameObject.Find(startObjectName).transform.position;

                        tmp1 = tmp;
                    }

                }

                //ボタンを離した処理
                if (Input.GetMouseButtonUp(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    //エンドの座標を取得
                    if (Physics.Raycast(ray, out hit))
                    {
                        endObjectName = hit.collider.gameObject.name;

                        tmp = GameObject.Find(endObjectName).transform.position;

                        tmp2 = tmp;

                        //重なったら上に出力させないようにする
                        if (startObjectName.Contains("masu") || endObjectName.Contains("masu"))
                        {

                        }
                        else
                        {
                            if (tmp1.x >= tmp2.x || tmp1.y >= tmp2.y)
                            {
                                tmp3.x = tmp1.x - tmp2.x;
                                tmp3.y = tmp1.y - tmp2.y;

                                for (int s = 0; tmp3.y >= s; s++)
                                {
                                    tmp.y = tmp1.y - s;

                                    for (int t = 0; tmp3.x >= t; t++)
                                    {
                                        tmp.x = tmp1.x - t;
                                        tmp.z = tmp1.z;

                                        //選択した座標を配列の要素番号に代入からのその上に床オブジェクトを配置
                                        field[(int)tmp.x, (int)tmp.y, (int)tmp.z + 1] = 1;

                                        //インデックスの値が1の時、cubeを生成
                                        if (field[(int)tmp.x, (int)tmp.y, (int)tmp.z + 1] == 1)
                                        {
                                            //Assetsからmasuを取得
                                            GameObject wall = Instantiate(masu);
                                            //オブジェクト生成時に"masu番号"に名前変更
                                            wall.name = "masu" + count;
                                            wall.transform.position = new Vector3(tmp.x, tmp.y, tmp.z + 1);
                                            count++;
                                            wall.tag = "1fRoom" + FirstfRoomCount;
                                        }
                                    }
                                }
                                for (int s = 0; tmp3.y >= s; s++)
                                {
                                    tmp.y = tmp1.y - s;

                                    for (int t = 0; tmp3.x >= t; t++)
                                    {
                                        tmp.x = tmp1.x - t;
                                        tmp.z = tmp1.z;

                                        if (field[(int)tmp.x + 1, (int)tmp.y, (int)tmp.z + 1] == 0 || field[(int)tmp.x - 1, (int)tmp.y, (int)tmp.z + 1] == 0 || field[(int)tmp.x, (int)tmp.y + 1, (int)tmp.z + 1] == 0 || field[(int)tmp.x, (int)tmp.y - 1, (int)tmp.z + 1] == 0)
                                        {
                                            // 選択した座標を配列の要素番号に代入からのその上に壁オブジェクトを配置
                                            field[(int)tmp.x, (int)tmp.y, (int)tmp.z + 2] = 1;
                                            field[(int)tmp.x, (int)tmp.y, (int)tmp.z + 3] = 1;
                                            field[(int)tmp.x, (int)tmp.y, (int)tmp.z + 4] = 1;

                                            //インデックスの値が1の時、cubeを生成
                                            for (int u = 2; u <= 4; u++)
                                            {
                                                if (field[(int)tmp.x, (int)tmp.y, (int)tmp.z + u] == 1)
                                                {
                                                    //Assetsからmasuを取得
                                                    GameObject wall = Instantiate(masu);

                                                    //オブジェクト生成時に"masu番号"に名前変更
                                                    wall.name = "masu" + count;
                                                    wall.transform.position = new Vector3(tmp.x, tmp.y, tmp.z + u);
                                                    count++;
                                                    wall.tag = "1fRoom" + FirstfRoomCount;
                                                    // 1番上の壁をListに格納
                                                    if (u == 4)
                                                    {
                                                        WallTopList.Add(wall);
                                                    }

                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                    FirstfRoomCount++;
                }
            }
            // 上のコードをコピペ
            else if (objsecond != null)
            {
                //ボタンを押した処理
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    //スタートの座標を取得
                    if (Physics.Raycast(ray, out hit))
                    {
                        startObjectName = hit.collider.gameObject.name;

                        tmp = GameObject.Find(startObjectName).transform.position;

                        tmp1 = tmp;
                    }

                }

                //ボタンを離した処理
                if (Input.GetMouseButtonUp(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    //エンドの座標を取得
                    if (Physics.Raycast(ray, out hit))
                    {
                        endObjectName = hit.collider.gameObject.name;

                        tmp = GameObject.Find(endObjectName).transform.position;

                        tmp2 = tmp;

                        //重なったら上に出力させないようにする
                        if (startObjectName.Contains("masu") || endObjectName.Contains("masu"))
                        {

                        }
                        else
                        {
                            if (tmp1.x >= tmp2.x || tmp1.y >= tmp2.y)
                            {
                                tmp3.x = tmp1.x - tmp2.x;
                                tmp3.y = tmp1.y - tmp2.y;

                                for (int s = 0; tmp3.y >= s; s++)
                                {
                                    tmp.y = tmp1.y - s;

                                    for (int t = 0; tmp3.x >= t; t++)
                                    {
                                        tmp.x = tmp1.x - t;
                                        tmp.z = tmp1.z;

                                        //選択した座標を配列の要素番号に代入からのその上に床オブジェクトを配置
                                        field[(int)tmp.x, (int)tmp.y, (int)tmp.z + 1] = 1;

                                        //インデックスの値が1の時、cubeを生成
                                        if (field[(int)tmp.x, (int)tmp.y, (int)tmp.z + 1] == 1)
                                        {
                                            //Assetsからmasuを取得
                                            GameObject wall = Instantiate(masu);
                                            //オブジェクト生成時に"masu番号"に名前変更
                                            wall.name = "masu" + count;
                                            wall.transform.position = new Vector3(tmp.x, tmp.y, tmp.z + 1);
                                            count++;
                                            wall.tag = "2fRoom" + SecondRoomCount;
                                        }
                                    }
                                }
                                for (int s = 0; tmp3.y >= s; s++)
                                {
                                    tmp.y = tmp1.y - s;

                                    for (int t = 0; tmp3.x >= t; t++)
                                    {
                                        tmp.x = tmp1.x - t;
                                        tmp.z = tmp1.z;

                                        if (field[(int)tmp.x + 1, (int)tmp.y, (int)tmp.z + 1] == 0 || field[(int)tmp.x - 1, (int)tmp.y, (int)tmp.z + 1] == 0 || field[(int)tmp.x, (int)tmp.y + 1, (int)tmp.z + 1] == 0 || field[(int)tmp.x, (int)tmp.y - 1, (int)tmp.z + 1] == 0)
                                        {
                                            // 選択した座標を配列の要素番号に代入からのその上に壁オブジェクトを配置
                                            field[(int)tmp.x, (int)tmp.y, (int)tmp.z + 2] = 1;
                                            field[(int)tmp.x, (int)tmp.y, (int)tmp.z + 3] = 1;
                                            field[(int)tmp.x, (int)tmp.y, (int)tmp.z + 4] = 1;

                                            //インデックスの値が1の時、cubeを生成
                                            for (int u = 2; u <= 4; u++)
                                            {
                                                if (field[(int)tmp.x, (int)tmp.y, (int)tmp.z + u] == 1)
                                                {
                                                    //Assetsからmasuを取得
                                                    GameObject wall = Instantiate(masu);

                                                    //オブジェクト生成時に"masu番号"に名前変更
                                                    wall.name = "masu" + count;
                                                    wall.transform.position = new Vector3(tmp.x, tmp.y, tmp.z + u);
                                                    count++;
                                                    wall.tag = "2fRoom" + SecondRoomCount;

                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                    SecondRoomCount++;
                }
            }
        }
        /*
         * (部屋生成処理)
         * -----------------------------------------------------------------
         */

        /*
         * 部屋削除の処理
         * -----------------------------------------------------------------
         */
        // 部屋削除ボタンが非表示のとき
        else
        {
            //ボタンを押した処理
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                //座標を取得
                if (Physics.Raycast(ray, out hit))
                {
                    startObjectName = hit.collider.gameObject.name;

                    tmp = GameObject.Find(startObjectName).transform.position;

                }
                GameObject tagname = GameObject.Find(startObjectName);

                GameObject[] deletetag = GameObject.FindGameObjectsWithTag(tagname.tag);

                foreach (GameObject gameObj in deletetag)
                {
                    gameObjName = gameObj.name;
                    TagPos = GameObject.Find(gameObjName).transform.position;

                    field[(int)TagPos.x, (int)TagPos.y, (int)TagPos.z] = 0;

                    Destroy(gameObj);
                }
            }
        }
        /*
         * (部屋削除処理)
         * -----------------------------------------------------------------
         */


        //2Fボタンが表示されているとき
        if (objsecond != null)
        {
            /*
             * 2階の地面生成の処理
             * -----------------------------------------------------------------
             */
            if (SecondStart == true)
            {
                SecondStart = false;

                foreach (GameObject delObj in WallTopList)
                {
                    Destroy(delObj);
                }
                //1と0を代入
                for (int i = 0; i < field.GetLength(0); i++)
                {
                    for (int j = 0; j < field.GetLength(1); j++)
                    {
                        if (field[i, j, 4] == 1)
                        {
                            field[i, j, 4] = 2;
                        }
                        else if (field[i, j, 4] == 0)
                        {
                            field[i, j, 4] = 1;
                        }
                    }
                }

                //各インデックスに代入された値をもとに生成するorしない

                for (int i = 0; i < field.GetLength(0); i++)
                {
                    for (int j = 0; j < field.GetLength(1); j++)
                    {
                        //インデックスの値が1の時、cubeを生成
                        if (field[i, j, 4] == 1)
                        {
                            GameObject wall = Instantiate(secondcube);
                            // wallの名前の変更 countによって一つ一つに番号をふる
                            wall.name = "secondcube" + count;
                            wall.transform.position = new Vector3(i, j, 4);
                            count++;
                        }//インデックスの値が2の時、cubeを生成
                        else if (field[i, j, 4] == 2)
                        {
                            GameObject wall = Instantiate(blackcube);
                            //wallの名前の変更 countによって一つ一つに番号をふる
                            wall.name = "blackcube" + blackcubecount;
                            wall.transform.position = new Vector3(i, j, 4);
                            blackcubecount++;
                        }
                    }
                }
                // SecondGrandDelTagに生成したキューブを追加
                GameObject[] SecondGrandDelTag = GameObject.FindGameObjectsWithTag("SecondCubeTag");
                SecondGrandDel = true;
            }
            /*
             * (2階の地面生成処理)
             * -----------------------------------------------------------------
             */
            if (objsecond != null)
            {
                if (SecondStart == true)
                {
                    SecondStart = false;
                    {

                    }
                }
            }
        }
    }
}
