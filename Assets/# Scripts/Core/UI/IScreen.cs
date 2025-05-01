using System;

public interface IScreen<T> where T : IScreen<T>
{
    event Action<T, bool> OnVisibleUpdated;

    bool IsVisible { get; }

    //void Initialize(); // TODO: мб пригодится
    void Show(bool isShow);
}
