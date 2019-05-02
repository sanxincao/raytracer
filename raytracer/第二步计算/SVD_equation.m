function x = SVD_equation(A,b)
%%������ֵ�ֽ�ķ���������Է����飬���÷���Ϊ
%%   x=SVD_equation(A,b)
%%����
%%   AΪ�������ϵ������bΪ��������Ҷ��
%%   xΪ������Ľ�.
   ep=1e-10; n = length(A); x=zeros(n,1);
   [U,S,V] = svd(A); sigma = diag(S);
   for i = 1 : n
       if abs(sigma(i)) >= ep
           x = x + (U(:,i)' * b)/sigma(i) * V(:,i);
       end
   end

end