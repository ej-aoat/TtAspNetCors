namespace MyWebApiServer;

/// <summary>
/// BFFが提供するAPIのハブを定義するインターフェースです。
/// </summary>
public interface IClientHubApi
{
    /// <summary>
    /// 同期化リクエストのレスポンストークンをフロントエンドに通知します。
    /// </summary>
    /// <returns></returns>
    Task AsyncNotification();

    /// <summary>
    /// 画面遷移をフロントエンドに通知します。
    /// </summary>
    /// <param name="transitionScreenNotification"></param>
    /// <returns></returns>
    Task TransitionScreen();

    /// <summary>
    /// データの更新をフロントエンドに通知します。
    /// </summary>
    /// <param name="updatePropertyNotification"></param>
    /// <returns></returns>
    Task UpdateProperty();
}