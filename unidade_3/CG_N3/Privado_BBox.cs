using OpenTK.Graphics.OpenGL;
using CG_Biblioteca;
using System;

namespace gcgcg
{
  internal class Privado_BBox : BBox
  {

    public Privado_BBox(double menorX = 0, double menorY = 0, double menorZ = 0, double maiorX = 0, double maiorY = 0, double maiorZ = 0 ): base(menorX, menorY, menorZ, maiorX, maiorY, maiorZ ) {
    }
    public bool validaDentro(Ponto4D ponto) {
        if (ponto.X <= obterMaiorX && ponto.X >= obterMenorX && ponto.Y <= obterMaiorY && ponto.Y >= obterMenorY) {
            return true;
        }
        return false;
    }
  }

}