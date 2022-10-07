/**
  Autor: Dalton Solano dos Reis
**/

#define CG_Debug
#define CG_OpenGL
// #define CG_DirectX

using OpenTK.Graphics.OpenGL;
using CG_Biblioteca;

namespace gcgcg
{
  internal class Spline : ObjetoGeometria
  {
    private int qntPontos;
    public Spline(char rotulo, Objeto paiRef, Ponto4D ptoEsquerdaBaixo, Ponto4D ptoEsquerdaCima, Ponto4D ptoDireitaCima, Ponto4D ptoDireitaBaixo, int qntPontos) : base(rotulo, paiRef)
    {
      this.qntPontos = qntPontos;
      base.PontosAdicionar(ptoEsquerdaBaixo);
      base.PontosAdicionar(ptoEsquerdaCima);
      base.PontosAdicionar(ptoDireitaCima);
      base.PontosAdicionar(ptoDireitaBaixo);
    }

    private Ponto4D calculaSpline(Ponto4D pto1, Ponto4D pto2, double valorPonto) {
      double ptoX = pto1.X + (pto2.X - pto1.X) * valorPonto / qntPontos;
      double ptoY = pto1.Y + (pto2.Y - pto1.Y) * valorPonto / qntPontos;

      return new Ponto4D(ptoX,ptoY);
    }

    protected override void DesenharObjeto()
    {
#if CG_OpenGL && !CG_DirectX
      Ponto4D pto1 =  pontosLista[0];
      Ponto4D pto2 =  pontosLista[1];
      Ponto4D pto3 =  pontosLista[2];
      Ponto4D pto4 =  pontosLista[3];




      GL.Begin(base.PrimitivaTipo);
      GL.Vertex2(pto1.X, pto1.Y);

      for (int i = 0; i < qntPontos; i++)
      {
        Ponto4D p1 = calculaSpline(pto1, pto2, i); 
        Ponto4D p2 = calculaSpline(pto2, pto3, i); 
        Ponto4D p3 = calculaSpline(pto3, pto4, i); 
        Ponto4D p12 = calculaSpline(p1, p2, i);
        Ponto4D p23 = calculaSpline(p2, p3, i);

        Ponto4D resultado = calculaSpline(p12, p23, i);

        GL.Vertex2(resultado.X, resultado.Y);
      }
      GL.Vertex2(pto4.X, pto4.Y);


      GL.End();
#elif CG_DirectX && !CG_OpenGL
    Console.WriteLine(" .. Coloque aqui o seu código em DirectX");
#elif (CG_DirectX && CG_OpenGL) || (!CG_DirectX && !CG_OpenGL)
    Console.WriteLine(" .. ERRO de Render - escolha OpenGL ou DirectX !!");
#endif
    }
    
    //TODO: melhorar para exibir não só a lista de pontos (geometria), mas também a topologia ... poderia ser listado estilo OBJ da Wavefrom
#if CG_Debug
    public override string ToString()
    {
      string retorno;
      retorno = "__ Objeto Retangulo: " + base.rotulo + "\n";
      for (var i = 0; i < pontosLista.Count; i++)
      {
        retorno += "P" + i + "[" + pontosLista[i].X + "," + pontosLista[i].Y + "," + pontosLista[i].Z + "," + pontosLista[i].W + "]" + "\n";
      }
      return (retorno);
    }
#endif

  }
}