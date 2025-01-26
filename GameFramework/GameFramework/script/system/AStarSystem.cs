// using GameFramework.script.component;
// using LitJson;
// using UnityEngine;
//
// namespace GameFramework.script.system;
//
// public class AStarSystem
// {
//     //降龙寻路控制====================================================================================
//     // List<RoadNode> roadNodeArr = null; //寻路数据
//
//     //触发该函数即可寻路
//     // public void OnPlayerClickNav(object udata)
//     // {
//     //     //判定角色动画状态是否处在Idle或者Run，除了这两个状态之外不能移动
//     //     if (this.state != (int)AnimState.Idle && this.state != (int)AnimState.Run)
//     //     {
//     //         return;
//     //     }
//     //
//     //     Vector2 position = (Vector2)udata;
//     //
//     //     Vector2 localPosition;
//     //     RectTransformUtility.ScreenPointToLocalPointInRectangle(this.mapOrigin, position, Camera.main, out localPosition);
//     //
//     //     float ox = this.gameObject.transform.localPosition.x;//目标原位置
//     //     float oy = this.gameObject.transform.localPosition.y;
//     //
//     //     float x = localPosition.x; //目标位置
//     //     float y = localPosition.y;
//     //
//     //     //初始化寻路数据
//     //     this.roadNodeArr = PathFindingAgent.instance.seekPath2(ox, oy, x, y);
//     //
//     //     //控制方向
//     //     float direction = x - ox;
//     //     if (this.gameObject.transform.Find("Spine").localScale.x > 0)
//     //     {
//     //         if (direction < 0f)
//     //         {
//     //             this.gameObject.transform.Find("Spine").localScale = new Vector3(-100, 80, 1);
//     //             this.gameObject.transform.Find("Spine").localPosition = new Vector3(-6, -5);
//     //             this.gameObject.transform.Find("Title").localPosition = new Vector3(3, 65.5f);
//     //             this.gameObject.transform.Find("Hp_frame").localPosition = new Vector3(3, 52f);
//     //             this.gameObject.transform.Find("Hp").localPosition = new Vector3(3, 52.6f);
//     //         }
//     //     }
//     //     if (this.gameObject.transform.Find("Spine").localScale.x < 0)
//     //     {
//     //         if (direction > 0f)
//     //         {
//     //             this.gameObject.transform.Find("Spine").localScale = new Vector3(100, 80, 1);
//     //             this.gameObject.transform.Find("Spine").localPosition = new Vector3(6, -5);
//     //             this.gameObject.transform.Find("Title").localPosition = new Vector3(-2, 65.5f);
//     //             this.gameObject.transform.Find("Hp_frame").localPosition = new Vector3(-2, 52f);
//     //             this.gameObject.transform.Find("Hp").localPosition = new Vector3(-2, 52.6f);
//     //         }
//     //     }
//     //     //传入目标位置
//     //     OnMapTouchDown(x, y);
//     // }
//
//     private void OnMapTouchDown(float x, float y)
//     {
//         //Vector2 mapPos;
//         //RectTransformUtility.ScreenPointToLocalPointInRectangle(mapOrigin, Input.mousePosition, Camera.main, out mapPos);
//         //NaviTo(mapPos.x, mapPos.y);
//         // NaviTo(x, y);
//     }
//
//     //降龙新增寻路
//     public volatile bool moving = false;
//     public float moveSpeed = 50;
//     protected float _moveAngle = 0;
//     protected List<RoadNode> _roadNodeArr = new List<RoadNode>();
//     //protected List<RoadNode> _roadNodeArr = this
//     protected int _nodeIndex = 0;
//     protected int _direction = 0;
//     
//     public void PlayerNavUpdate(Player player,TransformComponent transform)
//     {
//         Console.WriteLine("进来了");
//         if (player.animState == (int)AnimState.Idle) return;
//         Console.WriteLine("进来了2");
//         if (moving)
//         {
//             Console.WriteLine("进来了3");
//             // _roadNodeArr = this.roadNodeArr;
//             RoadNode nextNode = _roadNodeArr[_nodeIndex];
//
//            
//             // float dx = nextNode.px - transform.localPosition.x;
//             // float dy = nextNode.py - transform.localPosition.y;
//             float dx = nextNode.px - transform.ox;
//             float dy = nextNode.py - transform.oy;
//             Console.WriteLine("dx:"+dx+"dy: "+dy);
//             float speed = moveSpeed * (float)XTimer.instance.deltaTime / 1000;
//             Console.WriteLine((float)XTimer.instance.deltaTime);
//             Console.WriteLine("speed: "+speed);
//             Console.WriteLine(dx * dx + dy * dy > speed * speed);
//             if (dx * dx + dy * dy > speed * speed)
//             {
//                 if (_moveAngle == 0)
//                 {
//                     _moveAngle = Mathf.Atan2(dy, dx);
//
//                     int dire = (int)Mathf.Round((-_moveAngle + Mathf.PI) / (Mathf.PI / 4)); //算角度
//                                                                                             //direction = dire > 5 ? dire - 6 : dire + 2;
//                 }
//
//                 float xspeed = Mathf.Cos(_moveAngle) * speed;
//                 float yspeed = Mathf.Sin(_moveAngle) * speed;
//
//                 // Vector3 pos = transform.localPosition;
//                 
//                 
//                 // tx += xspeed;
//                 // ty += yspeed;
//                 //
//                 // transform.localPosition = pos;
//                 
//                 transform.ox += xspeed;
//                 transform.oy += yspeed;
//                 // player.Send();
//                 Console.WriteLine(transform.ox + " x " + transform.oy + " y " + transform.ox + " " + transform.oy);
//                 player.Send(transform);
//                 // NetManager.Send(player.state,transform);
//                 // PlayerManager.GetPlayer("gh").Send(transform);
//                 // Player player = new Player(); //TODO: 玩家？组件？如何决定同步的方式
//                 // NetManager.Send();
//                 
//                 //这里进行位移交互
//                 //SceneProxy.Instance.OnNavToPoint(pos);
//                 // if (pos == null)
//                 // {
//                 //     Debug.Log("pos null");
//                 // }
//                 // if (SceneProxy.Instance == null)
//                 // {
//                 //     Debug.Log("s null");
//                 // }
//                 //SceneProxy.Instance.OnPlayerSync(pos); //发送位置信息
//
//             }
//             else
//             {
//                 _moveAngle = 0;
//
//                 if (_nodeIndex == _roadNodeArr.Count - 1)
//                 {
//                     // Vector3 pos = transform.localPosition;
//                     //
//                     // pos.x = nextNode.px;
//                     // pos.y = nextNode.py;
//                     Stop();
//                 }
//                 else
//                 {
//                     Walk();
//                 }
//             }
//         }
//
//     }
//
//     /// <summary>
//     /// 根据路节点路径行走
//     /// </summary>
//     /// <param name="roadNodeArr"></param>
//     public void WalkByRoad(List<RoadNode> roadNodeArr)
//     {
//         _roadNodeArr = roadNodeArr;
//         _nodeIndex = 0;
//         _moveAngle = 0;
//
//         Walk();
//         Move();
//     }
//
//     private void Walk()
//     {
//         if (_nodeIndex < _roadNodeArr.Count - 1)
//         {
//             _nodeIndex++;
//             Console.WriteLine("Walk");
//         }
//         else
//         {
//
//         }
//     }
//
//     public void Move()
//     {
//         moving = true;
//         Console.WriteLine("Move");
//         Console.WriteLine(moving);
//         //state = CharacterState.walk;
//         // this.state = (int)AnimState.Run;
//         // anim.SetAnimation(0, "run", true);
//     }
//
//     public void Stop()
//     {
//         moving = false;
//         
//         //state = CharacterState.idle;
//         // this.state = (int)AnimState.Idle;
//         // anim.SetAnimation(0, "idle", true);
//     }
//
//     /// <summary>
//     /// 导航到目标位置
//     /// </summary>
//     /// <param name="targetX"></param>
//     /// <param name="targetY"></param>
//     public void NaviTo(TransformComponent transform)
//     {
//         Console.WriteLine(transform.ox + " " + transform.oy + " " + transform.tx + " " + transform.ty);
//         // List<RoadNode> roadNodeArr = PathFindingAgent.instance.SeekPath(transform.localPosition.x, transform.localPosition.y, targetX, targetY);
//         List<RoadNode> roadNodeArr = PathFindingAgent.instance.seekPath2(transform.ox, transform.oy, transform.tx, transform.ty);
//         
//         if (roadNodeArr.Count > 0)
//         {
//             WalkByRoad(roadNodeArr);
//             Console.WriteLine(roadNodeArr);
//             Console.WriteLine("寻到路径 count " + roadNodeArr.Count);
//         }
//         else
//         {
//             Console.WriteLine("目标不可到达");
//         }
//     }
//
//     public void Init(string filePath)
//     {
//         string jsonContent = File.ReadAllText(filePath);
//         MapData mapData = JsonMapper.ToObject<MapData>(jsonContent);
//         PathFindingAgent.instance.init(mapData);
//         Console.WriteLine(jsonContent);
//         Console.WriteLine(mapData);
//     }
// }