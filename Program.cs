using System;
using System.Collections.Generic; 
using System.IO;
using System.Linq;

namespace ConsoleAppFramework 
{
    internal class Program
    {
        private static void DecryptSwzFiles(string[] argsFromMain)
        {
            Console.WriteLine("\n--- Descriptografador de SWZ - By mt0(nobody.php) ---");
            List<string> filesToProcess = new List<string>();

            if (argsFromMain == null || argsFromMain.Length == 0)
            {
                Console.Write("Digite os NOMES dos arquivos .swz separados por espaço: ");
                string line = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(line))
                {
                    argsFromMain = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    Console.WriteLine("Nenhum arquivo fornecido.");
                    return;
                }
            }

            foreach (string argName in argsFromMain)
            {
                if (string.IsNullOrWhiteSpace(argName)) continue;
                filesToProcess.Add(Path.GetFullPath(argName.Trim('"')));
            }

            if (!filesToProcess.Any())
            {
                Console.WriteLine("Nenhum nome de arquivo válido para processar.");
                return;
            }

            Console.Write("Digite a chave de criptografia global (globalKey) para descriptografar: ");
            uint globalKey;
            if (!uint.TryParse(Console.ReadLine(), out globalKey))
            {
                Console.WriteLine("Chave global inválida.");
                return;
            }

            string dumpDirectory = Path.GetFullPath("Dump");
            Directory.CreateDirectory(dumpDirectory);

            foreach (string filePath in filesToProcess)
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"Arquivo não encontrado: {filePath}");
                    continue;
                }
                if (Path.GetExtension(filePath).ToLowerInvariant() != ".swz")
                {
                    Console.WriteLine($"Arquivo não é .swz (ignorado): {filePath}");
                    continue;
                }

                Console.WriteLine($"Processando arquivo: {filePath}...");
                string outputPattern = Path.Combine(dumpDirectory, Path.GetFileNameWithoutExtension(filePath) + "{0}.txt");

                try
                {
                    using (FileStream fileStream = File.OpenRead(filePath))
                    {
                        string[] decryptedStrings = BrawlhallaSWZ.Decrypt(fileStream, globalKey);
                        if (decryptedStrings.Length == 0)
                        {
                            Console.WriteLine("  Nenhuma string foi descriptografada. Verifique a chave ou o arquivo.");
                        }
                        else
                        {
                            Console.WriteLine($"  Descriptografadas {decryptedStrings.Length} entradas.");
                        }
                        for (int j = 0; j < decryptedStrings.Length; j++)
                        {
                            string outputPath = string.Format(outputPattern, j);
                            File.WriteAllText(outputPath, decryptedStrings[j]);
                            Console.WriteLine($"  Salvo: {outputPath}");
                        }
                    }
                    Console.WriteLine($"Arquivo {Path.GetFileName(filePath)} processado.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERRO ao processar o arquivo {filePath}: {ex.Message}");
                }
            }
            Console.WriteLine($"Processo de descriptografia concluído. Arquivos .txt salvos em: {dumpDirectory}");
        }

        private static void Main(string[] args)
        {
            Console.Title = "Ferramenta SWZ by mt0 (nobody.php)";

            string[] commandLineArgs = Environment.GetCommandLineArgs();

            if (commandLineArgs.Length > 1)
            {
                List<string> swzFileArgs = new List<string>();
                bool potentialSwzArgDetected = false;
                for (int i = 1; i < commandLineArgs.Length; i++)
                {
                    string arg = commandLineArgs[i];
                    if (arg.EndsWith(".swz", StringComparison.OrdinalIgnoreCase))
                    {
                        swzFileArgs.Add(arg);
                        potentialSwzArgDetected = true;
                    }
                }

                if (potentialSwzArgDetected)
                {
                    Console.WriteLine("iniciandO descriptografia...");
                    DecryptSwzFiles(swzFileArgs.ToArray());
                    Console.WriteLine("\nPressione qualquer tecla para sair.");
                    Console.ReadKey();
                    return;
                }
            }


            while (true)
            {
                Console.WriteLine("\nEscolha uma opção:");
                Console.WriteLine("1. Descriptografar arquivos .swz");
                Console.WriteLine("2. Compilar arquivos .txt para .swz");
                Console.WriteLine("3. Sair");
                Console.Write("Opção: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        DecryptSwzFiles(new string[0]);
                        break;
                    case "2":
                        SwzCompiler.CompileSwz();
                        break;
                    case "3":
                        Console.WriteLine("Expurgando-se BUM BUM TERRORISMO...");
                        return;
                    default:
                        Console.WriteLine("Opção inválida.");
                        break;
                }
                Console.WriteLine("\n---------------------------------------");
                Console.WriteLine("Compilação concluída. Pressione qualquer tecla para fechar.");
                Console.ReadKey();
                Console.Clear();
            }
        }
    }
}