using Faturamento.Api.Contracts;
using Faturamento.Application.NotasFiscais;
using Faturamento.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Faturamento.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotasFiscaisController : ControllerBase
{
    private readonly NotaFiscalService _notaFiscalService;

    public NotasFiscaisController(NotaFiscalService notaFiscalService)
    {
        _notaFiscalService = notaFiscalService;
    }

    [HttpPost]
    public async Task<IActionResult> Criar(CancellationToken cancellationToken)
    {
        var result = await _notaFiscalService.CriarNotaAsync(cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(nameof(ObterPorId), new { id = result.Value }, new { id = result.Value });
    }

    [HttpPost("{id:int}/itens")]
    public async Task<IActionResult> AdicionarItem(
        int id,
        [FromBody] AdicionarItemRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AdicionarItemNotaCommand(id, request.ProdutoId, request.Quantidade);
        var result = await _notaFiscalService.AdicionarItemAsync(command, cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObterPorId(int id, CancellationToken cancellationToken)
    {
        var nota = await _notaFiscalService.ObterPorIdAsync(id, cancellationToken);
        if (nota is null)
            return NotFound();

        return Ok(Mapear(nota));
    }

    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        var lista = await _notaFiscalService.ListarAsync(cancellationToken);
        return Ok(lista.Select(Mapear));
    }

    [HttpPost("{id:int}/imprimir")]
    public async Task<IActionResult> Imprimir(int id, CancellationToken cancellationToken)
    {
        var result = await _notaFiscalService.ImprimirAsync(id, cancellationToken);
        if (result.IsSuccess)
            return Ok(new { mensagem = "Nota impressa e estoque atualizado." });

        if (result.Error?.StartsWith(NotaFiscalService.PrefixoIndisponivel, StringComparison.OrdinalIgnoreCase) == true)
        {
            var detail = result.Error[NotaFiscalService.PrefixoIndisponivel.Length..].Trim();
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new ProblemDetails
            {
                Title = "Serviço de estoque indisponível",
                Detail = detail,
                Status = StatusCodes.Status503ServiceUnavailable
            });
        }

        return BadRequest(new { error = result.Error });
    }

    private static NotaFiscalResponse Mapear(Faturamento.Domain.Entities.NotaFiscal nota)
    {
        var itens = nota.Itens.Select(i => new ItemNotaResponse(i.ProdutoId, i.Quantidade)).ToList();
        var status = nota.Status == StatusNotaFiscal.Aberta ? "Aberta" : "Fechada";
        return new NotaFiscalResponse(nota.Id, nota.Numero, status, itens);
    }
}
