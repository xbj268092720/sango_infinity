/*
 * 文件名：GameState.cs
 * 描述：游戏状态类，管理游戏的状态切换
 * 创建日期：2026-03-27
 * 最后修改：2026-03-27
 */

namespace Sango.Game
{
    /// <summary>
    /// 游戏状态类，管理游戏的状态切换
    /// </summary>
    public class GameState : Singleton<GameState>
    {
        /// <summary>
        /// 游戏状态枚举
        /// </summary>
        public enum State : int
        {
            /// <summary>
            /// 无状态
            /// </summary>
            None = 0,
            /// <summary>
            /// 游戏开始菜单
            /// </summary>
            GAME_START_MENU = 1,
            /// <summary>
            /// 游戏中
            /// </summary>
            GAMEING = 2,
        }

        /// <summary>
        /// 当前状态
        /// </summary>
        int curState = 0;

        /// <summary>
        /// 获取当前状态
        /// </summary>
        /// <returns>当前状态值</returns>
        public int GetCurState()
        {
            return curState;
        }

        /// <summary>
        /// 切换游戏状态
        /// </summary>
        /// <param name="dest">目标状态</param>
        public void ChangeState(int dest)
        {
            if (curState != dest)
            {
                int last = curState;
                GameEvent.OnGameStateExit?.Invoke((int)curState, (int)dest);
                curState = dest;
                GameEvent.OnGameStateEnter?.Invoke((int)curState, (int)last);
            }
        }
    }
}
