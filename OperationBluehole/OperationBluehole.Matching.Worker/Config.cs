using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBluehole.Matching.Worker
{
	class Config
	{
		// 파티 당 멤버 수
		public const int MATCHING_PARTY_MEMBERS_NUM = 4;

		// 매칭 시 허용되는 레벨 차이
		public const int MATCHING_ALLOW_LEVEL_DIFF = 3;

		// 스레드 강제 종료 용 난이도 수치 (필요할까?)
		public const int MATCHING_THREAD_END_DIFFCULTY = int.MaxValue;
	}
}
