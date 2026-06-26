using MovieInfoBackend.DataModels;

namespace MovieInfoBackend.ViewModels;

public record TmdbCastViewModel
{    
    public TmdbCastViewModel(TmdbCastDataModel castDataModel)
    {
        this.ID = Guid.NewGuid();
        this.TmdbId = castDataModel.Id;
        this.Gender = (TmdbGenderType)castDataModel.Gender;
        this.Name = castDataModel.Name;
        this.OriginalName = castDataModel.OriginalName;
        this.ProfilePath = castDataModel.ProfilePath;
        this.Character = castDataModel.Character;
        this.BilledOrder = castDataModel.Order;
    }

    public Guid ID { get; init; }
    public int TmdbId { get; init; }
    public TmdbGenderType Gender { get; init; }
    public string Name { get; init; }
    public string OriginalName { get; init; }
    public string? ProfilePath { get; init; }
    public string Character { get; init; }
    public int BilledOrder { get; init; }

    public override string ToString()
    {
        return $"ID: {ID}\nTmdbId: {TmdbId}\nGender: {Gender}\nName: {Name}\nOriginalName: {OriginalName}\nProfilePath: {ProfilePath}\nCharacter: {Character}\nBilledOrder: {BilledOrder}";
    }
}
