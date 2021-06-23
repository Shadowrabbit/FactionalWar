// ******************************************************************
//       /\ /|       @file       GenStepThreat.cs
//       \ V/        @brief      生成步骤 威胁
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-23 17:59:43
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************
using Verse;

namespace SR.ModRimWorld.FactionalWar
{
    public class GenStepThreat : GenStep
    {
        public override int SeedPart => 1254675168;
        
        /// <summary>
        /// 生成
        /// </summary>
        /// <param name="map"></param>
        /// <param name="parms"></param>
        public override void Generate(Map map, GenStepParams parms)
        {
            //生成机械族或者虫族
            throw new System.NotImplementedException();
        }
    }
}
