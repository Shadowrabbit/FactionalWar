// ******************************************************************
//       /\ /|       @file       GenStepAirdropResource.cs
//       \ V/        @brief      生成步骤 空投资源
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-23 17:50:07
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************
using Verse;

namespace SR.ModRimWorld.FactionalWar
{
    public class GenStepAirdropResource : GenStep
    {
        public override int SeedPart => 546950704;
        /// <summary>
        /// 生成
        /// </summary>
        /// <param name="map"></param>
        /// <param name="parms"></param>
        public override void Generate(Map map, GenStepParams parms)
        {
            //生成珍贵资源
            //空投到中心
            throw new System.NotImplementedException();
        }
    }
}
