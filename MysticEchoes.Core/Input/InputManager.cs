namespace MysticEchoes.Core.Input;

public class InputManager
{
    public float Horizontal = 0.0f;
    public float Vertical = 0.0f;
    
    private readonly HashSet<Task> _tasks = new HashSet<Task>();
    
    public InputManager()
    {
    }

    public void AddTask(Task newTask)
    {
        _tasks.Add(newTask);
    }

    public void RemoveTask(Task taskToRemove)
    {
        _tasks.Remove(taskToRemove);
    }

    public bool IsTaskActive(Task taskToCheck)
    {
        return _tasks.Contains(taskToCheck);
    }

    public void Clear()
    {
        _tasks.Clear();
    }
}