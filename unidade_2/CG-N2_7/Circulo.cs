/**
  Autor: Dalton Solano dos Reis
**/

#define CG_Debug
#define CG_OpenGL
// #define CG_DirectX

using OpenTK.Graphics.OpenGL;
using CG_Biblioteca;
using System; 

namespace gcgcg
{
  internal class Ciurculo : ObjetoGeometria
  {
    private Ponto4D ptoCentral;
    private double raio;
    private bool visualizaCento;
    public Ciurculo(char rotulo, Objeto paiRef, Ponto4D ptoCentro, double raio, int qtdePontos, bool desenhaCento = false) : base(rotulo, paiRef)
    {
      this.visualizaCento = desenhaCento;
      this.ptoCentral = ptoCentro;
      this.raio = raio;
      for (int i = 0; i < qtdePontos; i++)
      {
        base.PontosAdicionar(Matematica.GerarPtosCirculo(i*(360/qtdePontos), raio) + ptoCentro);
        base.PontosAdicionar(Matematica.GerarPtosCirculo((i+1)*(360/qtdePontos), raio) + ptoCentro);
      }
    }

    public Ponto4D pegaPontosCirculoPeloAngulo(double angulo) {
      return Matematica.GerarPtosCirculo(angulo, this.raio) + this.ptoCentral;
    }

    public double distanciaEuclediana(Ponto4D pto) {
      return Math.Sqrt(Math.Pow(pto.X - ptoCentral.X, 2) + Math.Pow(pto.Y - ptoCentral.Y, 2));
    }
    protected override void DesenharObjeto()
    {
#if CG_OpenGL && !CG_DirectX
      GL.Begin(base.PrimitivaTipo);
      foreach (Ponto4D pto in pontosLista)
      {
        GL.Vertex2(pto.X, pto.Y);
      }
      GL.End();
      if (this.visualizaCento) {
        GL.Begin(PrimitiveType.Points);
        GL.Vertex2(this.ptoCentral.X, this.ptoCentral.Y);
        GL.End();
      }
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