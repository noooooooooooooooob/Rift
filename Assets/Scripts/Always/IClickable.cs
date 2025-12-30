/// <summary>
/// 클릭 가능한 오브젝트를 위한 인터페이스
/// 이 인터페이스를 구현한 오브젝트는 ClickController로부터 클릭 이벤트를 받을 수 있음
/// 필수 조건: 오브젝트에 Collider 컴포넌트 필요 (레이캐스트 감지용)
/// </summary>
public interface IClickable
{
    /// <summary>
    /// 오브젝트가 클릭되었을 때 호출되는 메서드
    /// </summary>
    void OnClick();
}