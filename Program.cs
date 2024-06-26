﻿using ConsumindoApiMetadados.Entities;
using ConsumindoApiMetadados.Entities.Service;
using Newtonsoft.Json.Linq;


Console.WriteLine("-----------------------------------------------------");
Console.WriteLine("------------- Consumindo API Metadados --------------");

Console.WriteLine("Informe o CPF:");
string cpfParametro = Console.ReadLine();

dynamic pessoa = await PessoaService.GetCadastroPreAdmissao(cpfParametro);

Console.WriteLine(pessoa);

dynamic jsonData = JArray.Parse($@"{pessoa}");

foreach (var item in jsonData)
{
    //item.pessoa.id = null;
    //Removendo o propriedade Pessoa.ID
    ((JObject)item.pessoa).Remove("id");

    item.pessoa.nome = "Samuel Matos";
    item.pessoa.nomeCompleto = "Samuel Rodrigues de Matos";
    item.pessoa.cpf = "00000000000";
}

dynamic pessoaId = await PessoaService.PostCadastroPreAdmissao(jsonData);

Console.WriteLine("ID do Usuario Cadastrado");
if (pessoaId != null)
{
    Console.WriteLine(pessoaId);
}
else
{
    Console.WriteLine("O retorno do POST foi null. Verificar.");
}

Console.WriteLine("-----------------FIM----------------");
Console.WriteLine("------------------------------------");












