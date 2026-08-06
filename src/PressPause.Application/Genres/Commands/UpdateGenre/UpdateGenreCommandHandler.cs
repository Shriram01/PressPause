using MediatR;
using PressPause.Application.Common.Interfaces;
using PressPause.Application.Genres.Commands.UpdateGenre;

namespace PressPause.Application.Genres.Commands.UpdateGenre;

public sealed class UpdateGenreCommandHandler
    : IRequestHandler<UpdateGenreCommand, bool>
{
    private readonly IGenreRepository _repository;

    public UpdateGenreCommandHandler(
        IGenreRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(
        UpdateGenreCommand request,
        CancellationToken cancellationToken)
    {
        var genre = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (genre is null)
        {
            return false;
        }

        genre.Update(
            request.Name,
            request.Slug);

        _repository.Update(genre);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return true;
    }
}