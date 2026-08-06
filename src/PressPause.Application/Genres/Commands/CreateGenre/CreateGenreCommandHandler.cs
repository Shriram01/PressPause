using MediatR;
using PressPause.Application.Common.Interfaces;
using PressPause.Domain.Entities;

namespace PressPause.Application.Genres.Commands.CreateGenre;

public sealed class CreateGenreCommandHandler
    : IRequestHandler<CreateGenreCommand, Guid>
{
    private readonly IGenreRepository _repository;

    public CreateGenreCommandHandler(
        IGenreRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(
        CreateGenreCommand request,
        CancellationToken cancellationToken)
    {
        var genre = new Genre(
            request.Name,
            request.Slug);

        await _repository.AddAsync(
            genre,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return genre.Id;
    }
}