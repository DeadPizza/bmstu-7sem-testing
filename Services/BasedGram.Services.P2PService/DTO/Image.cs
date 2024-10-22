namespace BasedGram.Services.P2PService.DTO;

public class Image
{
    public Image(string name, Stream image)
    {
        Name = name;
        _Image = image;
    }

    public string Name { get; set; }
    public Stream _Image { get; set; }
}
