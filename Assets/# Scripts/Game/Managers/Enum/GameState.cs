public enum GameState
{
    /// <summary>
    /// Обычная игра
    /// </summary>
    Played,
    /// <summary>
    /// Диалог
    /// </summary>
    Dialogue,
    /// <summary>
    /// За минуту до охоты
    /// </summary>
    Suspense,
    /// <summary>
    /// Переход к охоте
    /// </summary>
    PursuitTransition,
    /// <summary>
    /// Погоня
    /// </summary>
    Pursuit,
    /// <summary>
    /// Пауза
    /// </summary>
    Paused
}
