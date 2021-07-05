// ******************************************************************
//       /\ /|       @file       BaseSingleTon.cs
//       \ V/        @brief      单例基类
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-07-02 10:58:46
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************
namespace SR.ModRimWorld.FactionalWar
{
    public class BaseSingleTon<T> where T : class, new()
    {
        public static T Instance => Inner.InternalInstance;

        private static class Inner
        {
            internal static readonly T InternalInstance = new T();
        }
    }
}
