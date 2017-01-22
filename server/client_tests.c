#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <netdb.h>
#include <unistd.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

const int maxn = 15;

void write_loop(int fd, char * buffer, int n)
{
    printf("pisze %s\n", buffer);
    int i = 0;
    while(i < n)
    {
        i += write(fd, buffer+i, n-i);
    }

    //printf("napisalem\n");
}

void read_loop(int fd, char * buffer, int n)
{
    //printf("czytam\n");
    int i = 0;
    while(i < n)
    {
        i += read(fd, buffer+i, n-i);
    }
    //printf("odczytalem %s\n", buffer);
}

int main(int argc, char **argv)
{
    if(argc<3) 
	{
		 fprintf(stderr, "enter\n");
		return 1;
	}

    int fd = socket(PF_INET, SOCK_STREAM, 0);
    if(fd == -1) {
        fprintf(stderr, "socket\n");
    }
    struct sockaddr_in saddr;
    saddr.sin_family = PF_INET;
    saddr.sin_port = htons(atoi(argv[2]));
    struct hostent *host = gethostbyname(argv[1]);
    memcpy(&saddr.sin_addr.s_addr, host->h_addr_list[0], host->h_length);

    // saddr.sin_addr.s_addr = inet_addr(argv[1]); //tamtą metodą nie chciało mi z localhostem połączyć
    if(connect(fd, (struct sockaddr*) &saddr, sizeof(saddr))!=0) {
        fprintf(stderr, "connect\n");
    }
    //printf("connect correct\n");


	char buffer[15]={0};
    
	while(1){
		bzero(buffer, sizeof(buffer));
		fgets(buffer, sizeof(buffer), stdin);
		uint32_t i=0;
		for(i;i<sizeof(buffer);i++)if(buffer[i]=='\n')buffer[i]='\0'; // Pozbywamy się '\n' na końcu.
		
		if(buffer[0] == 'e' && buffer[1] == 'n' && buffer[2] == 'd')
			break;
		printf("a\n");
		write_loop(fd, buffer, maxn);
		bzero(buffer, maxn);
		read_loop(fd, buffer, maxn);

		printf("%s\n", buffer);
	}
    close(fd);

    return 0;
}
