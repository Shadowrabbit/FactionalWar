// ******************************************************************
//       /\ /|       @file       GenStepFactionContention.cs
//       \ V/        @brief      生成步骤 派系争夺
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-23 18:03:37
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************
using Verse;

namespace SR.ModRimWorld.FactionalWar
{
    public class GenStepFactionContention : GenStep
    {
        public override int SeedPart => 546950705;
        /// <summary>
        /// 生成
        /// </summary>
        /// <param name="map"></param>
        /// <param name="parms"></param>
        public override void Generate(Map map, GenStepParams parms)
        {
            //生成两个相互敌对的派系 设置集群AI互相攻击并争夺资源
            throw new System.NotImplementedException();
        }
    }
}
