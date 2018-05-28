using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoritmoGenetico
{
    class Program
    {
        private static Random rnd = new Random();
        const int TAM = 500;
        const int NUM_LINHAS_COLUNAS = 8;
        const int APTIDAO_MAXIMA = (NUM_LINHAS_COLUNAS - 1) * NUM_LINHAS_COLUNAS + 1;

        static int[][,] populacao = new int[TAM][,];
        static int[] aptidoes = new int[TAM];
        static int somaAptidoes;

        static void Inicializar()
        {
            int[,] individuo;

            for (int i = 0; i < TAM; i++)
            {
                individuo = RetornarIndividuo();
                populacao[i] = individuo;
                aptidoes[i] = RetornarAptidao(individuo);
            }

            somaAptidoes = RetornarSomaAptidoes();

        }

        private static int RetornarSomaAptidoes()
        {
            int soma = aptidoes.Sum();

            Console.WriteLine($" Soma das aptidões: {soma}");

            return soma;

        }

        private static int RetornarIndiceSolucao()
        {
            for (int i = 0; i < TAM; i++)
                if (aptidoes[i] == APTIDAO_MAXIMA)
                    return i;

            return -1;
        }

        private static void SelecionarIndividuos(out int indice1, out int indice2)
        {
            indice1 = RetornarIndiceSelecao(somaAptidoes);

            indice2 = RetornarIndiceSelecao(somaAptidoes);

            while(indice1 == indice2)
                indice1 = RetornarIndiceSelecao(somaAptidoes);
        }

        private static int RetornarIndiceSelecao(int somaAptidoes)
        {
            int seta = rnd.Next(somaAptidoes);

            int i = 0;
            int soma = aptidoes[i];

            while(soma < seta)
                soma += aptidoes[++i];

            return i;
        }

        private static int[,] RetornarIndividuo()
        {
            int[,] individuo = new int[NUM_LINHAS_COLUNAS, NUM_LINHAS_COLUNAS];

            for (int i = 0; i < NUM_LINHAS_COLUNAS; i++)
            {
                individuo[i, rnd.Next(NUM_LINHAS_COLUNAS)] = 1;
            }


            return individuo;
        }

        private static int RetornarAptidao(int[,] individuo)
        {
            int cont = 0;

            for (int i = 0; i < NUM_LINHAS_COLUNAS; i++)
            {
                for (int j = 0; j < NUM_LINHAS_COLUNAS; j++)
                {
                    if (individuo[i, j] == 1)
                    {
                        cont += RetornarConflitosColuna(individuo, i, j);
                        cont += RetornarConflitosDiagonais(individuo, i, j);
                    }
                }
            }

            return APTIDAO_MAXIMA - cont;

        }

        private static int RetornarConflitosDiagonais(int[,] individuo, int linha, int coluna)
        {
            int cont = 0;

            int i, j;

            for (i = linha - 1, j = coluna - 1; i >= 0 && j >= 0; i--, j--)
                if (individuo[i, j] == 1)
                    cont++;

            for (i = linha - 1, j = coluna + 1; i >= 0 && j < NUM_LINHAS_COLUNAS; i--, j++)
                if (individuo[i, j] == 1)
                    cont++;

            for (i = linha + 1, j = coluna - 1; i < NUM_LINHAS_COLUNAS && j >= 0; i++, j--)
                if (individuo[i, j] == 1)
                    cont++;

            for (i = linha + 1, j = coluna + 1; i < NUM_LINHAS_COLUNAS && j < NUM_LINHAS_COLUNAS; i++, j++)
                if (individuo[i, j] == 1)
                    cont++;


            return cont;

        }

        private static int RetornarConflitosColuna(int[,] individuo, int linha, int coluna)
        {
            int cont = 0;

            for (int i = 0; i < NUM_LINHAS_COLUNAS; i++)
                if (i != linha)
                    if (individuo[i, coluna] == 1)
                        cont++;

            return cont;
        }

        static void Main(string[] args)    
        {
            int indiceSolucao = -1;
            int geracao = 0;

            Inicializar();

            do
            {

                indiceSolucao = RetornarIndiceSolucao();

                if (indiceSolucao == -1)
                {
                    GerarNovaPopulacao();
                }


                Console.WriteLine("Geração: {0}", ++geracao);

            } while (indiceSolucao == -1);

            Console.WriteLine("\nSolução encontrada:");
            Imprimir(populacao[indiceSolucao]);

            Console.ReadKey();
        }

        private static void Imprimir(int[,] individuo)
        {
            for (int i = 0; i < NUM_LINHAS_COLUNAS; i++)
            {
                for (int j = 0; j < NUM_LINHAS_COLUNAS; j++)
                {
                    Console.Write(" {0}", individuo[i,j]);
                }
                Console.WriteLine();
            }
        }

        private static void GerarNovaPopulacao()
        {
            int[][,] novaPopulacao = new int[TAM][,];
            int[] novasAptidoes = new int[TAM];
            int indiceIndiv1, indiceIndiv2;
            int[,] novo1, novo2;

            for (int i = 0; i < TAM; i+=2)
            {
                SelecionarIndividuos(out indiceIndiv1, out indiceIndiv2);

                Cruzar(indiceIndiv1, indiceIndiv2, out novo1, out novo2);

                novaPopulacao[i] = novo1;
                novaPopulacao[i + 1] = novo2;

                novasAptidoes[i] = RetornarAptidao(novo1);
                novasAptidoes[i + 1] = RetornarAptidao(novo2);
            }

            populacao = novaPopulacao;
            aptidoes = novasAptidoes;
            somaAptidoes = RetornarSomaAptidoes();

        }

        private static void Cruzar(int indiceIndiv1, int indiceIndiv2, out int[,] novo1, out int[,] novo2)
        {
            int corte = rnd.Next(NUM_LINHAS_COLUNAS - 1);

            novo1 = new int[NUM_LINHAS_COLUNAS, NUM_LINHAS_COLUNAS];
            novo2 = new int[NUM_LINHAS_COLUNAS, NUM_LINHAS_COLUNAS];

            for (int i = 0; i < NUM_LINHAS_COLUNAS; i++)
            {
                for (int j = 0; j < NUM_LINHAS_COLUNAS; j++)
                {
                    if (i <= corte)
                    {
                        novo1[i, j] = populacao[indiceIndiv1][i, j];
                        novo2[i, j] = populacao[indiceIndiv2][i, j];
                    }
                    else
                    {
                        novo1[i, j] = populacao[indiceIndiv2][i, j];
                        novo2[i, j] = populacao[indiceIndiv1][i, j];
                    }
                }
            }

            Mutacao(novo1);
            Mutacao(novo2);

        }

        private static void Mutacao(int[,] individuo)
        {
            int mut = rnd.Next(TAM);
            int linha, coluna;

            if (mut == 0)
            {
                linha = rnd.Next(NUM_LINHAS_COLUNAS);
                coluna = rnd.Next(NUM_LINHAS_COLUNAS);

                for (int k = 0; k < NUM_LINHAS_COLUNAS; k++)
                {
                    if (k == coluna)
                        individuo[linha, k] = 1;
                    else
                        individuo[linha, k] = 0;
                }

            }
        }
    }
}
