using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OperationBluehole.Server.Modules
{
    // 가입, 로그인 등 사용자 인증 작업을 처리
    public class UserModule
    {
        // 가입
        // 이메일, 비밀번호, 

        // 로그인
        // 이메일과 비밀번호를 전송받아서
        // 저장된 값과 비교해보고
        // 같으면 쿠키 발급

        // 인증
        // 기본적으로 쿠키를 발급하자
        // 사용자의 모든 요청에 대해서 쿠키를 확인하고
        // 만약 쿠키가 유효하지 않다면 로그인을 하게 만든다
        // 쿠키값을 확인하면 플레이어의 id를 알 수 있게 한다
        // 

        // 확인할 것들
        // 쿠키 생성
        // 쿠키 확인
        // 

        // 디비 저장 방식
        // 유저 - 메일 주소, 암호화 된 비밀번호, 캐릭터 id, user id, ban list
        // 캐릭터 - 캐릭터 id, 캐릭터들 정보, user id도 알아야 하려나
    }
}