using MediatR;
using PressPause.Application.Common.Interfaces;

namespace PressPause.Application.Genres.Commands.DeleteGenre;

public sealed class DeleteGenreCommandHandler
    : IRequestHandler<DeleteGenreCommand, bool>
{
    private readonly IGenreRepository _repository;

    public DeleteGenreCommandHandler(
        IGenreRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(
        DeleteGenreCommand request,
        CancellationToken cancellationToken)
    {
        var genre = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (genre is null)
        {
            return false;
        }

        _repository.Delete(genre);

        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }
}