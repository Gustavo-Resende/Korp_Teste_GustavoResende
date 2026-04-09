using Estoque.Api.Contracts;
using Estoque.Application.Produtos;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly ProdutoService _produtoService;

    public ProdutosController(ProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    [HttpPost]
    public async Task<IActionResult> Criar(
        [FromBody] CriarProdutoCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _produtoService.CriarAsync(request, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        var p = result.Value!;
        return CreatedAtAction(
            nameof(ObterPorId),
            new { id = p.Id },
            new ProdutoResponse(p.Id, p.Codigo, p.Descricao, p.Saldo));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObterPorId(int id, CancellationToken cancellationToken)
    {
        var produto = await _produtoService.ObterPorIdAsync(id, cancellationToken);
        if (produto is null)
            return NotFound();

        return Ok(new ProdutoResponse(produto.Id, produto.Codigo, produto.Descricao, produto.Saldo));
    }

    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        var lista = await _produtoService.ListarAsync(cancellationToken);
        return Ok(lista.Select(p => new ProdutoResponse(p.Id, p.Codigo, p.Descricao, p.Saldo)));
    }

    [HttpPost("deduzir")]
    public async Task<IActionResult> Deduzir(
        [FromBody] DeduzirEstoqueRequest request,
        CancellationToken cancellationToken)
    {
        var itens = request.Itens.Select(i => (i.ProdutoId, i.Quantidade)).ToList();
        var result = await _produtoService.DeduzirItensAsync(itens, cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }
}
