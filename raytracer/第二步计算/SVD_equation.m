function x = SVD_equation(A,b)
%%用奇异值分解的方法求解线性方程组，调用方法为
%%   x=SVD_equation(A,b)
%%其中
%%   A为方程组的系数矩阵，b为方程组的右端项；
%%   x为方程组的解.
   ep=1e-10; n = length(A); x=zeros(n,1);
   [U,S,V] = svd(A); sigma = diag(S);
   for i = 1 : n
       if abs(sigma(i)) >= ep
           x = x + (U(:,i)' * b)/sigma(i) * V(:,i);
       end
   end

end