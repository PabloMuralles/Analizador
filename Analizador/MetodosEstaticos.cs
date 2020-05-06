using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Analizador
{
    class MetodosEstaticos
    {
        /// <summary>
        /// Metodo para realizar la tokenizacion de la cadena entrada
        /// </summary>
        /// <param name="CadenaEntrada">Cadena de entrada a evaluar</param>
        /// <param name="TokensNoExpresiones">La lista de tokens sin que sean expresiones regulares </param>
        /// <returns>Una cola con la cadena tokenizada</returns>
        public Queue<string> Tokenizar(string CadenaEntrada, List<string> TokensNoExpresiones)
        {
            try
            {
                Queue<string> tokens = new Queue<string>();
                string[] elements = CadenaEntrada.Split(new[] { "\r\n", "\r", "\n", "\t", " " }, StringSplitOptions.None);
                foreach (string element in elements)
                {
                    if (!string.IsNullOrWhiteSpace(element) && !string.IsNullOrEmpty(element))
                    {
                        List<string> find = TokensNoExpresiones.FindAll(x => element.Contains(x));
                        if (find.Count() == 0)
                        {
                            tokens.Enqueue(element);
                        }
                        else
                        {
                            var greatestElement = find.OrderByDescending(x => x.Length).FirstOrDefault();

                            int index = element.IndexOf(greatestElement);
                            string newElement = string.Empty;
                            if (index == 0)
                            {
                                tokens.Enqueue(element.Substring(0, greatestElement.Length));
                                newElement = element.Remove(0, greatestElement.Length);
                            }
                            else
                            {
                                tokens.Enqueue(element.Substring(0, index));
                                newElement = element.Remove(0, index);
                            }
                            while (newElement.Length > 0)
                            {
                                find = TokensNoExpresiones.FindAll(x => newElement.Contains(x));
                                if (find.Count() == 0)
                                {
                                    tokens.Enqueue(newElement);
                                    newElement = string.Empty;
                                }
                                else
                                {
                                    index = newElement.IndexOf(newElement);
                                    if (index == 0)
                                    {
                                        tokens.Enqueue(newElement.Substring(0, newElement.Length));
                                        newElement = newElement.Remove(0, newElement.Length);
                                    }
                                    else
                                    {
                                        tokens.Enqueue(newElement.Substring(0, index));
                                        newElement = newElement.Remove(0, index);
                                    }
                                }
                            }
                        }
                    }

                }

                return tokens;

            }
            catch (Exception)
            {


            }
            return default;
        }
        /// <summary>
        /// Metodo para poder saber a que token le pertenece cada parte de la cadena o tambien las reservadas
        /// </summary>
        /// <param name="TokensCadena">una cola de los tokens de la cadena recibida por el usuario</param>
        /// <param name="TokensGramatica">son todos los tokens de la gramatica con las reservads</param>
        /// <param name="Sets">los sets con sus rangos para poder verificar con cada token que es una expresion regular</param>
        /// <returns>una lista con los tokens de la cadena de entrada con cada uno de sus respectivos token o reservas</returns>
        public List<string> MostrarTokens(Queue<string> TokensCadena, Dictionary<string, string> TokensGramatica, Dictionary<string, List<string>> Sets)
        {


            try
            {
                var ListaMostrarTokens = new List<string>();


                while (TokensCadena.Count != 0)
                {
                    var Token = TokensCadena.Dequeue();

                    if (TokensGramatica.Values.Contains(Token))
                    {
                        var TokenPertenece = string.Empty;
                        TokenPertenece += TokensGramatica.FirstOrDefault(x => x.Value == Token).Key.ToString();
                        TokenPertenece += $"= {Token }";
                        ListaMostrarTokens.Add(TokenPertenece);


                    }
                    else
                    {
                        if (Token.Length == 1)
                        {
                            var TokenPertenece = string.Empty;
                            foreach (var item in Sets)
                            {
                                var end = false;
                                if (item.Value.Contains(Token))
                                {
                                    foreach (var token in TokensGramatica)
                                    {
                                        if (token.Value.Contains(item.Key))
                                        {
                                            TokenPertenece += token.Key.ToString();
                                            TokenPertenece += $"= {Token}";
                                            ListaMostrarTokens.Add(TokenPertenece);
                                            end = true;
                                            break;

                                        }

                                    }
                                    if (end == true)
                                    {
                                        break;
                                    }

                                }
                            }

                        }
                        else
                        {

                            var Aux = string.Empty;
                            var Concatenacion = string.Empty;
                            var contador = 1;
                            foreach (var caracter in Token)
                            {
                                var end = false;
                                var TokenPertenece = string.Empty;
                                foreach (var item in Sets)
                                {
                                    if (item.Value.Contains(caracter.ToString()))
                                    {
                                        foreach (var token in TokensGramatica)
                                        {
                                            if (token.Value.Contains(item.Key))
                                            {
                                                if (Aux.Length == 0)
                                                {
                                                    Aux = token.Key;
                                                    Concatenacion += caracter;
                                                    if (contador == Token.Length)
                                                    {
                                                        TokenPertenece = $"{Aux} = {Concatenacion}";
                                                        ListaMostrarTokens.Add(TokenPertenece);

                                                        Concatenacion = string.Empty;
                                                        TokenPertenece = string.Empty;
                                                    }
                                                    end = true;
                                                    break;

                                                }
                                                else if (Aux == token.Key)
                                                {
                                                    Concatenacion += caracter;
                                                    if (Token.Length == Concatenacion.Length)
                                                    {
                                                        TokenPertenece = $"{Aux} = {Concatenacion}";
                                                        ListaMostrarTokens.Add(TokenPertenece);

                                                        Concatenacion = string.Empty;
                                                        TokenPertenece = string.Empty;
                                                    }
                                                    end = true;
                                                    break;

                                                }
                                                else if (Aux != token.Key)
                                                {
                                                    TokenPertenece = $"{Aux} = {Concatenacion}";
                                                    ListaMostrarTokens.Add(TokenPertenece);
                                                    Aux = token.Key;
                                                    Concatenacion = string.Empty;
                                                    TokenPertenece = string.Empty;
                                                }
                                                end = true;
                                                break;



                                            }

                                        }
                                        if (end == true)
                                        {
                                            break;
                                        }

                                    }
                                    else
                                    {

                                        if (TokensGramatica.Values.Contains(caracter.ToString()))
                                        {
                                            TokenPertenece = string.Empty;
                                            TokenPertenece = $"{TokensGramatica.FirstOrDefault(x => x.Value == caracter.ToString()).Key.ToString()} = {caracter}";
                                            ListaMostrarTokens.Add(TokenPertenece);
                                            TokenPertenece = string.Empty;
                                            break;
                                        }

                                    }

                                }
                                contador++;


                            }
                            if (Concatenacion.Length != 0)
                            {
                                var TokenPertenece = $"{Aux} = {Concatenacion}";
                                ListaMostrarTokens.Add(TokenPertenece);

                                Concatenacion = string.Empty;
                                TokenPertenece = string.Empty;

                            }
                        }
                    }
                }
                return ListaMostrarTokens;

            }
            catch (Exception)
            {


            }
            return default;


        }


    }
}
