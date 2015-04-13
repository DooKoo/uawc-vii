Для запуску додатку виконайте такі команди:

vagrant up --provision

Підключіться по ssh до віртуальної машини(username:vagrant, password:vagrant), 
та виконайте наступні команди:

cd /vagrant/app
kvm upgrade
kpm restore
k kestrel

сервер працює за адресою: localhost:8888