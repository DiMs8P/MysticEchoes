namespace MazeGeneration.Exceptions;

public class InvalidGenerationException : Exception
{
    public override string Message => "Unexpected exception while generating map. You can retry with other seed";

    public InvalidGenerationException(GenerationConfig config)
    {
        Data.Add("config", config);
    }
}