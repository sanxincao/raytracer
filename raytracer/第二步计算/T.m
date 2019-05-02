function [d_xyz] = T() 
%m为测站数，n为公共点
m=5;n=48;
A=zeros(m*n,4*m+3*n);
b=zeros(n*m,1);
yy=zeros(1,4*m+3*n);
L=zeros(m,n);
rr=zeros(m,n);

X = xlsread('StandPoints1', 'Sheet1', 'A1:A5');
Y = xlsread('StandPoints1', 'Sheet1', 'B1:B5');
Z = xlsread('StandPoints1', 'Sheet1', 'C1:C5');
d = xlsread('StandPoints1', 'Sheet1', 'D1:D5');

x = xlsread('data2', 'Sheet1', 'A1:A48');
y = xlsread('data2', 'Sheet1', 'B1:B48');
z = xlsread('data2', 'Sheet1', 'C1:C48');
r1 = xlsread('data2', 'Sheet1', 'E1:E48');
r2 = xlsread('data2', 'Sheet1', 'F1:F48');
r3 = xlsread('data2', 'Sheet1', 'G1:G48');
r4 = xlsread('data2', 'Sheet1', 'H1:H48');
r5 = xlsread('data2', 'Sheet1', 'I1:I48');




kx=zeros(n,1);
ky=zeros(n,1);
kz=zeros(n,1);
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
xx_value=100;
k=1;
while k<=1;
    for j=1:m
        for i=1:n
            L(j,i)=sqrt((X(j)-x(i))^2+(Y(j)-y(i))^2+(Z(j)-z(i))^2);
        end
    end
    for j=1:m
        for i=1:n
            A((j-1)*n+i,4*j-3)= -1;
            A((j-1)*n+i,4*j-2)=(X(j)-x(i))/L(j,i);
            A((j-1)*n+i,4*j-1)=(Y(j)-y(i))/L(j,i);
            A((j-1)*n+i,4*j)=(Z(j)-z(i))/L(j,i);
        end
    end
    %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    for j=1:m
        for i=1:n
            A((j-1)*n+i,4*m+(3*i-2))=(x(i)-X(j))/L(j,i);
            A((j-1)*n+i,4*m+(3*i-1))=(y(i)-Y(j))/L(j,i);
            A((j-1)*n+i,4*m+(3*i))=(z(i)-Z(j))/L(j,i);
        end
    end
    %对系数矩阵b赋值
    bb=zeros(n,m);
    for i =1:n
        bb(i,1) = r1(i,1)+ d(1,1) - L(1,i);
        bb(i,2) = r2(i,1)+ d(2,1) - L(2,i);
        bb(i,3) = r3(i,1)+ d(3,1) - L(3,i);
        bb(i,4) = r4(i,1)+ d(4,1) - L(4,i);
        bb(i,5) = r5(i,1)+ d(5,1) - L(5,i);
        
    end
    for j=1:m
        for i=1:n
            b(i+(j-1)*n,1)=bb(i,j);
        end
    end
    %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    [U,S,V]=svd(A);
    AA=U*S*V';
    qqq=U'*b;
    qq=pinv(S)*qqq;
    xx=V*qq;
    xx_value=norm(xx,2);
    %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    %%%%%%%迭代求解
    h=1;
    for i=1:4:4*m-3
        d(h,1)=d(h,1)+xx(i);
        h=h+1;
    end
    h=1;
    for i=2:4:4*m-2
        X(h,1)=X(h,1)+xx(i);
        h=h+1;
    end
    h=1;
    for i=3:4:4*m-1
        Y(h,1)=Y(h,1)+xx(i);
        h=h+1;
    end
    h=1;
    for i=4:4:4*m
        Z(h,1)=Z(h,1)+xx(i);
        h=h+1;
    end
    %%%%%%%%
    h=1;
    for i=4*m+1:3:4*m+3*n-2
        kx(h)=xx(i);
        x(h,1)=x(h,1)+xx(i);
        h=h+1;
    end
    h=1;
    for i=4*m+2:3:4*m+3*n-1
        ky(h)=xx(i);
        y(h,1)=y(h,1)+xx(i);
        h=h+1;
    end
    h=1;
    for i=4*m+3:3:4*m+3*n
        kz(h)=xx(i);
        z(h,1)=z(h,1)+xx(i);
        h=h+1;
    end
    k=k+1;
end
p=zeros(n,3);
for i=1:n
    p(i,1)=x(i);
    p(i,2)=y(i);
    p(i,3)=z(i);
end
d_xyz(:,1)=kx;
d_xyz(:,2)=ky;
d_xyz(:,3)=kz;
