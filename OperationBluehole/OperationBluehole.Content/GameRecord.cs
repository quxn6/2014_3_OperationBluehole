using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OperationBluehole.Content
{
    public struct TargetAffected
    {
        public Character target;
        public GaugeType gaugeType;
        public int value;
    }

    public class GameRecord
    {
        // 참조할 정보 : path, battle log, looted items, looted gold, looted exp

        // 과정
        public List<Int2D> pathfinding = new List<Int2D>();
        public List<List<TurnInfo>> battleLog = new List<List<TurnInfo>>();

        // 전리품
        public List<Item> lootedItems = new List<Item>();
        public int lootedGold = 0;
        public int lootedExp = 0;
    }

    public struct TurnInfo
    {
        public int turn;
        public byte inTurnSeq; // 동일 턴 행동시 순서 표시용 // 사실 리스트 순서가 있으니 필요 없을지도... 혹시나 싶어서 넣어봄
        public Character src;
        public SkillId skillId;
        public ItemCode itemCode;
        public List<TargetAffected> targets; // 대상별 적용 수치
    }

    public class BattleInfo
    {
        public List<TurnInfo> turnInfos { get; private set; }
        public int curTurn { get; private set; }
        public byte curInTurnSeq { get; private set; }
        private TurnInfo curTurnInfo;
        private byte curAffectCount;

        public BattleInfo()
        {
            turnInfos = new List<TurnInfo>();
            curTurn = 0;
            curInTurnSeq = 0;
        }

        // 현재 턴 설정
        public void SetTurn(int turn)
        {
            if (curTurn != turn)
            {
                curTurn = turn;
                curInTurnSeq = 0;
            }
        }

        // 현재 턴 기록 시작
        public void StartTurnInfo()
        {
            curTurnInfo = new TurnInfo();
            curTurnInfo.turn = curTurn;
            curTurnInfo.inTurnSeq = ++curInTurnSeq;
            curAffectCount = 0;
        }

        // 행동 기록
        public void RecordAction(Character src, SkillId skillId, ItemCode itemCode, List<Character> targets)
        {
            curTurnInfo.src = src;
            curTurnInfo.skillId = skillId;
            curTurnInfo.itemCode = itemCode;
            curTurnInfo.targets = new List<TargetAffected>();

            if (targets == null) // 즉, 행동 하지 않았을 때
            {
                turnInfos.Add(curTurnInfo);
                curAffectCount = 0;
                return;
            }

            foreach (var target in targets)
            {
                var tta = new TargetAffected();
                tta.target = target;
                curTurnInfo.targets.Add(tta);
            }
        }

        // 각 타겟에 적용된 수치 기록
        public void RecordAffect(Character target, GaugeType gaugeType, int value)
        {
            var targetAffect = curTurnInfo.targets.First(chr => chr.target == target);
            targetAffect.gaugeType = gaugeType;
            targetAffect.value = value;
            ++curAffectCount;

            if (curTurnInfo.targets.Count == curAffectCount)
            {
                turnInfos.Add(curTurnInfo);
                curAffectCount = 0;
            }
        }
    }
}
