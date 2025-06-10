using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConsoleAppFramework 
{
    public static class SwzCompiler
    {
        public static void CompileSwz()
        {
            Console.WriteLine("\n--- Compilador de SWZ ---");

            uint globalKey;
            string originalSwzPath;
            string txtFolderPath;
            string outputSwzPath;

            
            while (true)
            {
                Console.Write("Digite a chave de criptografia global (globalKey): ");
                if (uint.TryParse(Console.ReadLine(), out globalKey))
                {
                    break;
                }
                Console.WriteLine("Entrada inválida. A chave global deve ser um número inteiro não negativo.");
            }

            while (true)
            {
                Console.Write("Digite o NOME do arquivo .swz original (ex: macaco.swz): ");
                string originalSwzName = Console.ReadLine().Trim('"');
                if (string.IsNullOrWhiteSpace(originalSwzName))
                {
                    Console.WriteLine("Nome do arquivo .swz original não pode ser vazio.");
                    continue;
                }
                originalSwzPath = Path.GetFullPath(originalSwzName); 
                if (File.Exists(originalSwzPath))
                {
                    break;
                }
                Console.WriteLine($"Arquivo .swz original não encontrado em: {originalSwzPath}.\n Oia de novo o nome e deixa de ser burro.");
            }

            while (true)
            {
                Console.Write("Digite o NOME da pasta contendo os arquivos .txt: ");
                string txtFolderName = Console.ReadLine().Trim('"');
                if (string.IsNullOrWhiteSpace(txtFolderName))
                {
                    Console.WriteLine("A Pasta não pode estar vazia.");
                    continue;
                }
                txtFolderPath = Path.GetFullPath(txtFolderName); 
                if (Directory.Exists(txtFolderPath))
                {
                    break;
                }
                Console.WriteLine($"Pasta de arquivos .txt não encontrada em: {txtFolderPath}. Verifique o nome e tente novamente.");
            }

            Console.Write("Digite o NOME para o novo arquivo .swz de saída (ex: ArquivoCompilado.swz): ");
            string outputSwzName = Console.ReadLine().Trim('"');
            if (string.IsNullOrWhiteSpace(outputSwzName))
            {
                Console.WriteLine("Nome de saída inválido. Usando 'Compilado.swz' no diretório atual.");
                outputSwzName = "Compilado.swz";
            }
            outputSwzPath = Path.GetFullPath(outputSwzName); 

            try
            {
                string outputDirectory = Path.GetDirectoryName(outputSwzPath);
                if (!string.IsNullOrEmpty(outputDirectory) && !Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                    Console.WriteLine($"Pasta criada: {outputDirectory}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO ao tentar criar o diretório de saída: {ex.Message}");
                return;
            }


            uint originalSeed = 0;
            try
            {
                using (FileStream fs = File.OpenRead(originalSwzPath))
                {
                    BrawlhallaSWZ.ReadUInt32BE(fs);
                    originalSeed = BrawlhallaSWZ.ReadUInt32BE(fs);
                }
                Console.WriteLine($"Seed original lido do arquivo '{Path.GetFileName(originalSwzPath)}': {originalSeed}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO ao ler o seed do arquivo .swz original ({originalSwzPath}): {ex.Message}");
                return;
            }

            List<string> stringEntries = new List<string>();
            try
            {
                Console.WriteLine($"Lendo arquivos .txt de: {txtFolderPath}");
                var txtFiles = Directory.GetFiles(txtFolderPath, "*.txt")
                                        .Select(path => new
                                        {
                                            Path = path,
                                            FileName = Path.GetFileNameWithoutExtension(path) 
                                        })
                                        .Select(file =>
                                        {
                                            Match match = Regex.Match(file.FileName, @"(\d+)$");
                                            return new
                                            {
                                                file.Path,
                                                Order = match.Success ? int.Parse(match.Groups[1].Value) : -1, 
                                                OriginalName = file.FileName 
                                            };
                                        })
                                        .Where(file => file.Order != -1) 
                                        .OrderBy(file => file.Order)
                                        .ToList();

                if (!txtFiles.Any())
                {
                    Console.WriteLine("Nenhum arquivo .txt com numeração no final (ex: nome0.txt, nome1.txt) encontrado na pasta especificada.");
                    return;
                }

                Console.WriteLine($"Arquivos .txt encontrados e ordenados para compilação ({txtFiles.Count} arquivos):");
                foreach (var txtFile in txtFiles)
                {
                    string content = File.ReadAllText(txtFile.Path);
                    stringEntries.Add(content);
                    Console.WriteLine($"  Lido: {Path.GetFileName(txtFile.Path)} ({content.Length} caracteres)");
                }
            }
            catch (FormatException ex) 
            {
                Console.WriteLine($"ERRO: Formato de número inválido no nome de um arquivo .txt");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO ao ler os arquivos .txt da pasta {txtFolderPath}: {ex.Message}");
                return;
            }

            if (!stringEntries.Any())
            {
                Console.WriteLine("Nenhuma string foi carregada dos arquivos .txt. Abortando compilação.");
                return;
            }

            try
            {
                Console.WriteLine("Criptografando e compilando o arquivo .swz...");
                byte[] outputBytes = BrawlhallaSWZ.Encrypt(originalSeed, globalKey, stringEntries.ToArray());

                File.WriteAllBytes(outputSwzPath, outputBytes);
                Console.WriteLine($"Novo arquivo .swz salvo em: {Path.GetFullPath(outputSwzPath)} ({outputBytes.Length} bytes)");
                Console.WriteLine("Compilação concluída com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO durante a criptografia ou ao salvar o arquivo ({outputSwzPath}): {ex.Message}");
            }
        }
    }
}